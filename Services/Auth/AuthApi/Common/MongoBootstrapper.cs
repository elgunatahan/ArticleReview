using AuthApi.Documents;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace AuthApi.Common
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
            CreateUserIndexesAndDefaultUsers();
        }

        private void RegisterIgnoreExtraElementsBehaviour()
        {
            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);
        }

        private void CreateUserIndexesAndDefaultUsers()
        {
            var collection = _mongoClient
                .GetDatabase("AuthDB")
                .GetCollection<User>("User");

            collection.Indexes.CreateOne(new CreateIndexModel<User>(
                new IndexKeysDefinitionBuilder<User>()
                    .Ascending(c => c.Username),
                new CreateIndexOptions<User> { Background = true }));

            var adminFilter = Builders<User>.Filter.Eq(x => x.Username, "Admin");

            var adminUser = collection.Find(adminFilter).SingleOrDefault();

            if (adminUser == null)
            {
                User user = new User()
                {
                    Id = Guid.NewGuid(),
                    Version = 1,
                    Username = "Admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("kloia"),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "AuthApi",
                    IsDeleted = false,
                    Role = "ADMIN"
                };

                collection.InsertOne(user);
            }


            var memberFilter = Builders<User>.Filter.Eq(x => x.Username, "Member");

            var memberUser = collection.Find(memberFilter).SingleOrDefault();

            if (memberUser == null)
            {
                User user = new User()
                {
                    Id = Guid.NewGuid(),
                    Version = 1,
                    Username = "Member",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("kloia"),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "AuthApi",
                    IsDeleted = false,
                    Role = "MEMBER"
                };

                collection.InsertOne(user);
            }



            var onlyArticleApiUserFilter = Builders<User>.Filter.Eq(x => x.Username, "ArticleApiUser");

            var articleApiUser = collection.Find(onlyArticleApiUserFilter).SingleOrDefault();

            if (articleApiUser == null)
            {
                User user = new User()
                {
                    Id = Guid.NewGuid(),
                    Version = 1,
                    Username = "ArticleApiUser",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("kloia"),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "AuthApi",
                    IsDeleted = false,
                    Role = "ONLYARTICLEAPI"
                };

                collection.InsertOne(user);
            }


            var onlyReviewApiUserFilter = Builders<User>.Filter.Eq(x => x.Username, "ReviewApiUser");

            var reviewApiUser = collection.Find(onlyReviewApiUserFilter).SingleOrDefault();

            if (reviewApiUser == null)
            {
                User user = new User()
                {
                    Id = Guid.NewGuid(),
                    Version = 1,
                    Username = "ReviewApiUser",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("kloia"),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "AuthApi",
                    IsDeleted = false,
                    Role = "ONLYREVIEWAPI"
                };

                collection.InsertOne(user);
            }
        }
    }
}
