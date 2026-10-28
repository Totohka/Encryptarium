namespace Encryptarium.Domain.Model.Entities;

public class xStorageRole : xBaseEntity
{
    public Guid StorageId { get; set; }
    public eStorage Storage { get; set; }
    public int RoleId { get; set; }
    public eRole Role { get; set; }
}
