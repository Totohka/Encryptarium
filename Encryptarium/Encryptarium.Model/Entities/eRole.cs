using Encryptarium.Domain.Enum;

namespace Encryptarium.Domain.Model.Entities;

public class eRole : BaseEntity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public RoleType RoleType { get; set; }
    public HashSet<xUserRole> UserRoles { get; set; }
    public HashSet<xSecretRole> SecretRoles { get; set; }
    public HashSet<xStorageRole> StorageRoles { get; set; }
    public HashSet<cFunctionalRuleSecret> FunctionalRulesSecret { get; set; }
    public HashSet<cFunctionalRuleStorage> FunctionalRulesStorage { get; set; }
    public override bool Equals(object? obj)
    {
        if (obj is eRole role) 
            return Id == role.Id;
        return false;
    }
    public override int GetHashCode() => Id.GetHashCode();
}
