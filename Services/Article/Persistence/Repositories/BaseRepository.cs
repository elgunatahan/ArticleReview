using Application.Common.CurrentUser;
using Application.Exceptions;
using Domain.Entities;
using Domain.ValueObjects;
using MongoDB.Driver;
using Persistence.Interfaces;

namespace Persistence.Repositories
{
    public class BaseRepository<D, E>
            where D : IDocument
            where E : BaseEntity
    {
        protected readonly IMongoCollection<D> _collection;
        protected readonly string _userName;

        public BaseRepository(IMongoClient mongoClient, ICurrentUser currentUser)
        {
            var database = mongoClient.GetDatabase("ArticleDB");
            _collection = database.GetCollection<D>(typeof(E).Name);
            _userName = currentUser.Username;
        }
        protected async Task InsertOneWithVersioning(D document, E entity, CancellationToken cancellationToken)
        {
            document.Version = IdentityValueObject.NextVersionIncrement;
            document.CreatedAt = DateTime.UtcNow;
            document.CreatedBy = _userName;
            await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);

            entity.IdentityObject.SetNextVersionNumber();
        }

        protected async Task<UpdateResult> UpdateOneWithVersioning(E entity, UpdateDefinition<D> update, CancellationToken cancellationToken)
        {
            var updateDefinitions = new List<UpdateDefinition<D>> { update };

            UpdateDefinition<D> versionUpdateDefinition;

            if (!string.IsNullOrWhiteSpace(_userName))
            {
                versionUpdateDefinition = Builders<D>.Update
                    .Inc("Version", IdentityValueObject.NextVersionIncrement)
                    .Set("UpdatedAt", DateTime.UtcNow)
                    .Set("UpdatedBy", _userName);
            }
            else
            {
                versionUpdateDefinition = Builders<D>.Update
                   .Inc("Version", IdentityValueObject.NextVersionIncrement)
                   .Set("UpdatedAt", DateTime.UtcNow);
            }

            updateDefinitions.Add(versionUpdateDefinition);

            var combinedUpdateDefinition = Builders<D>.Update.Combine(updateDefinitions);

            var builder = Builders<D>.Filter;
            var filter = builder.Eq("_id", entity.IdentityObject.Id) & builder.Eq("Version", entity.IdentityObject.Version);

            var result = await _collection.UpdateOneAsync(filter, combinedUpdateDefinition, cancellationToken: cancellationToken);

            if (!result.IsAcknowledged || result.ModifiedCount == 0)
            {
                throw new WrongExpectedVersionException(result.ToString());
            }

            entity.IdentityObject.SetNextVersionNumber();

            return result;
        }

        protected async Task InsertManyWithVersioning(IEnumerable<D> documents)
        {
            foreach (var document in documents)
            {
                document.CreatedAt = DateTime.UtcNow;
                document.CreatedBy = _userName;
                document.Version = IdentityValueObject.NextVersionIncrement;
            }

            await _collection.InsertManyAsync(documents);
        }

        protected async Task UpdateManyWithVersioning(List<UpdateOneModel<D>> requestedModels, List<D> documents)
        {
            List<UpdateOneModel<D>> updateOneModels = new List<UpdateOneModel<D>>();
            for (int i = 0; i < requestedModels.Count(); i++)
            {
                var filter = requestedModels[i].Filter;
                filter &= Builders<D>.Filter.Eq(x => x.Version, documents[i].Version);

                UpdateDefinition<D> updateDefinition;

                if (!string.IsNullOrWhiteSpace(_userName))
                {
                    updateDefinition = requestedModels[i].Update
                        .Inc("Version", IdentityValueObject.NextVersionIncrement)
                        .Set("UpdatedAt", DateTime.UtcNow)
                        .Set("UpdatedBy", _userName);
                }
                else
                {
                    updateDefinition = requestedModels[i].Update
                        .Inc("Version", IdentityValueObject.NextVersionIncrement)
                        .Set("UpdatedAt", DateTime.UtcNow);
                }

                var updateModel = new UpdateOneModel<D>(filter, updateDefinition);
                updateOneModels.Add(updateModel);

            }
            var result = await _collection.BulkWriteAsync(updateOneModels);
            
            var requestCount = documents.Select(x => x.Id).Distinct().Count();

            if (!result.IsAcknowledged || result.ModifiedCount != requestCount)
            {
                throw new WrongExpectedVersionException(result.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter">If null, non filter</param>
        /// <param name="sortDefinition">If null, sort by CreatedAt</param>
        /// <returns></returns>
        protected async Task<(int totalCount, IReadOnlyList<D> readOnlyList)> QueryByPage(int page, int pageSize, FilterDefinition<D> filter = null, SortDefinition<D> sortDefinition = null)
        {
            if (page < 1)
                page = 1;

            filter ??= Builders<D>.Filter.Empty;
            sortDefinition ??= Builders<D>.Sort.Descending(x => x.CreatedAt);

            var countFacet = AggregateFacet.Create("count",
                PipelineDefinition<D, AggregateCountResult>.Create(new[]
                {
                PipelineStageDefinitionBuilder.Count<D>()
                }));

            var dataFacet = AggregateFacet.Create("data",
                PipelineDefinition<D, D>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Sort(sortDefinition),
                    PipelineStageDefinitionBuilder.Skip<D>((page - 1) * pageSize),
                    PipelineStageDefinitionBuilder.Limit<D>(pageSize),
                }));

            var aggregation = await _collection.Aggregate()
                .Match(filter)
                .Facet(countFacet, dataFacet)
                .ToListAsync();

            var count = aggregation.First()
                .Facets.First(x => x.Name == "count")
                .Output<AggregateCountResult>()
                ?.FirstOrDefault()
                ?.Count ?? 0;


            var data = aggregation.First()
                .Facets.First(x => x.Name == "data")
                .Output<D>();

            return ((int)count, data);
        }
    }
}