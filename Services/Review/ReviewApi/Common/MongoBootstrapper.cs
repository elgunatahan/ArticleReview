using Domain.Entities;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Persistence.Documents;

namespace ReviewApi.Common
{
    public class MongoBootstrapper
    {
        private readonly IMongoClient _mongoClient;
        public MongoBootstrapper(IMongoClient client)
        {
            _mongoClient = client;
        }

        public void Migrate()
        {
            RegisterIgnoreExtraElementsBehaviour();
            CreateReviewIndexes();
        }

        private void RegisterIgnoreExtraElementsBehaviour()
        {
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);
        }

        private void CreateReviewIndexes()
        {
            var collection = _mongoClient
                .GetDatabase("ReviewDB")
                .GetCollection<ReviewDocument>(nameof(Review));

            collection.Indexes.CreateOne(new CreateIndexModel<ReviewDocument>(
                new IndexKeysDefinitionBuilder<ReviewDocument>()
                    .Ascending(c => c.ArticleId),
                new CreateIndexOptions<ReviewDocument> { Background = true }));
        }
    }
}