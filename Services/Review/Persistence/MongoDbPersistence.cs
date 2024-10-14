using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Persistence.Documents;

namespace Persistence
{
    public static class MongoDbPersistence
    {
        public static void Configure()
        {
            BsonClassMap.RegisterClassMap<ReviewDocument>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });

            var pack = new ConventionPack
            {
                new IgnoreExtraElementsConvention(true)
            };
            ConventionRegistry.Register("My Solution Conventions", pack, t => true);
        }
    }
}
