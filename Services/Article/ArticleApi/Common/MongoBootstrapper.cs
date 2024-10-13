using Domain.Entities;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace ArticleApi.Common
{
    public class MongoBootstrapper
    {
        private readonly IMongoClient _mongoClient;
        private readonly string _dbName;
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
                .GetCollection<Article>(nameof(Article));

            collection.Indexes.CreateOne(new CreateIndexModel<Article>(
                new IndexKeysDefinitionBuilder<Article>()
                    .Ascending(c => c.Title),
                new CreateIndexOptions<Article> { Background = true }));
        }
    }
}