using MongoDB.Bson.Serialization.Attributes;

namespace AuthApi.Documents
{
    public class User
    {
        [BsonId]
        public Guid Id { get; set; }
        public int Version {  get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        
        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; }
    }
}
