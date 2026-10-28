namespace Encryptarium.Domain.Model.Entities;

public class xSecretRole : xBaseEntity
{
    public Guid SecretId { get; set; }
    public eSecret Secret { get; set; }
    public int RoleId { get; set; }
    public eRole Role { get; set; }
}
