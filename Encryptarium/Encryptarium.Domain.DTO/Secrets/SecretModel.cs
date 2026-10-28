using Encryptarium.Domain.Enum;

namespace Encryptarium.Domain.DTO.Secrets
{
    public class SecretModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public SecretTypes SecretType { get; set; }
        public Guid StorageId { get; set; }
        public SecretValueBaseDto? Value { get; set; }
    }
}
