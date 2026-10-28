using Encryptarium.Domain.Enum;

namespace Encryptarium.Domain.Model.Entities;

public class eSecret : BaseEntity<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public SecretTypes SecretType { get; set; }
    public Guid StorageId { get; set; }
    public eStorage Storage { get; set; }
    public rSecretText? SecretText { get; set; }
    public rSecretFile? SecretFile { get; set; }
    public rSecretUserPass? SecretUserPass { get; set; }
    public HashSet<xSecretRole> SecretRoles { get; set; } = new HashSet<xSecretRole>();
}
