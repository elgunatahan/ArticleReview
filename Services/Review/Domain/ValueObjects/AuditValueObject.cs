namespace Domain.ValueObjects
{
    public class AuditValueObject
    {
        public string CreatedBy { get; }
        public DateTime? CreatedAt { get; }
        public string UpdatedBy { get; }
        public DateTime? UpdatedAt { get; }

        public AuditValueObject(string createdBy)
        {
            CreatedBy = createdBy;
        }

        public AuditValueObject(AuditValueObject audit, string updatedBy)
        {
            CreatedBy = audit.CreatedBy;
            CreatedAt = audit.CreatedAt;
            UpdatedBy = updatedBy;
        }

        public AuditValueObject(string createdBy, DateTime createdAt, string updatedBy, DateTime? updatedAt)
        {
            CreatedBy = createdBy;
            CreatedAt = createdAt;
            UpdatedBy = updatedBy;
            UpdatedAt = updatedAt;
        }
    }
}
