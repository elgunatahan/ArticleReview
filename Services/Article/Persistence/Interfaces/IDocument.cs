using MongoDB.Bson.Serialization.Attributes;

namespace Persistence.Interfaces
{
    public interface IDocument
    {
        [BsonId]
        Guid Id { get; set; }
        int Version { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        string CreatedBy { get; set; }
        string UpdatedBy { get; set; }
        bool IsDeleted { get; set; }
    }
}
