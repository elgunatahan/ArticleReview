using Domain.ValueObjects;

namespace Domain.Entities
{
    public abstract class BaseEntity
    {
        public BaseEntity(IdentityValueObject identityObject)
        {
            IdentityObject = identityObject ?? new IdentityValueObject();
        }

        public IdentityValueObject IdentityObject { get; }

        public AuditValueObject Audit { get; protected set; }
    }
}
