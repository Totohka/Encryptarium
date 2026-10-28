namespace Encryptarium.Domain.Model.Entities;

public class xUserRole : xBaseEntity
{
    public int RoleId { get; set; }
    public eRole Role { get; set; }
    public int UserId { get; set; }
    public eUser User { get; set; }
}
