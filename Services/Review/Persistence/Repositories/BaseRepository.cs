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
            var database = mongoClient.GetDatabase("ReviewDB");
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
    }
}