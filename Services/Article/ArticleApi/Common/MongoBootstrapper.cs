using Domain.Entities;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Persistence.Documents;

namespace ArticleApi.Common
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
            CreateArticleIndexes();
        }

        private void RegisterIgnoreExtraElementsBehaviour()
        {
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);
        }

        private void CreateArticleIndexes()
        {
            var collection = _mongoClient
                .GetDatabase("ArticleDB")
                .GetCollection<ArticleDocument>(nameof(Article));

            collection.Indexes.CreateOne(new CreateIndexModel<ArticleDocument>(
                new IndexKeysDefinitionBuilder<ArticleDocument>()
                    .Ascending(c => c.Title),
                new CreateIndexOptions<ArticleDocument> { Background = true }));
        }
    }
}