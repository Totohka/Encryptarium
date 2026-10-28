using Encryptarium.Domain.Enum;

namespace Encryptarium.Domain.Model.Entities;

public class eStorage : BaseEntity<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int CountSecret { get; set; } = 30;
    public HashSet<eSecret> Secrets { get; set; }
    public StorageTypes StorageType { get; set; }
    public HashSet<xStorageRole> StorageRoles { get; set; } = new HashSet<xStorageRole>();
}
