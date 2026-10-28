using Microsoft.AspNetCore.Identity;

namespace Encryptarium.Domain.Model.Entities;

public class eUser : IdentityUser<int>
{
    public string? RefreshToken { get; set; }
    public HashSet<xUserRole> UserRoles { get; set; }
}
