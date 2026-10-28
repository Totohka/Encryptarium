namespace Encryptarium.Domain.Model.Entities;

public class cFunctionalRuleSecret : BaseEntity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public HashSet<eRole> Roles { get; set; } = new HashSet<eRole>();
}
