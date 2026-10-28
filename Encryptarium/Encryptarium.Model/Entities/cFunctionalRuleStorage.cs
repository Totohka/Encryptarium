namespace Encryptarium.Domain.Model.Entities;

public class cFunctionalRuleStorage : BaseEntity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public HashSet<eRole> Roles { get; set; }
}
