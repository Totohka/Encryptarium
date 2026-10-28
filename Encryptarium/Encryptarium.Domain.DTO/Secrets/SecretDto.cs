using Encryptarium.Domain.Enum;

namespace Encryptarium.Domain.DTO.Secrets
{
    public class SecretDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public SecretTypes SecretType { get; set; }
        public SecretValueBaseDto? Value { get; set; }
    }
}
