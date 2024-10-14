using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ReviewApi.Common
{

    public class EdmModelSerializer : IBsonSerializer<IEdmModel>
    {
        public Type ValueType => typeof(IEdmModel);

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, IEdmModel value)
        {
            context.Writer.WriteStartDocument();
            // Example of serializing entity sets
            foreach (var entitySet in value.EntityContainer.EntitySets())
            {
                context.Writer.WriteName(entitySet.Name);
                context.Writer.WriteStartDocument();
                // Add relevant properties here
                context.Writer.WriteEndDocument();
            }
            context.Writer.WriteEndDocument();
        }

        public IEdmModel Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var entitySets = new List<EntitySetConfiguration>();
            // Read and reconstruct the EdmModel from BSON
            context.Reader.ReadStartDocument();
            while (context.Reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                // Read each entity set and reconstruct
            }
            context.Reader.ReadEndDocument();

            // Create an EdmModel from the reconstructed entity sets
            var builder = new ODataConventionModelBuilder();
            foreach (var entitySet in entitySets)
            {
                // Add entity sets back to the builder
            }
            return (EdmModel)builder.GetEdmModel();
        }


        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var entitySets = new List<EntitySetConfiguration>();
            // Read and reconstruct the EdmModel from BSON
            context.Reader.ReadStartDocument();
            while (context.Reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                // Read each entity set and reconstruct
            }
            context.Reader.ReadEndDocument();

            // Create an EdmModel from the reconstructed entity sets
            var builder = new ODataConventionModelBuilder();
            foreach (var entitySet in entitySets)
            {
                // Add entity sets back to the builder
            }
            return (EdmModel)builder.GetEdmModel();
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            context.Writer.WriteStartDocument();
            // Example of serializing entity sets
            foreach (var entitySet in ((IEdmModel)value).EntityContainer.EntitySets())
            {
                context.Writer.WriteName(entitySet.Name);
                context.Writer.WriteStartDocument();
                // Add relevant properties here
                context.Writer.WriteEndDocument();
            }
            context.Writer.WriteEndDocument();
        }
    }
}
