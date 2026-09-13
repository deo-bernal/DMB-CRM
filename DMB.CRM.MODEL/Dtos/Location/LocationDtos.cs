namespace Dmb.Crm.Model.Dtos.Location;

public class ReadLocationDto
{
    public Guid Id { get; set; }
    public Guid AgencyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Timezone { get; set; } = "Asia/Manila";
    public bool IsActive { get; set; }
    public string Role { get; set; } = Dmb.Crm.Model.Roles.User;
}

public class LocationStatsDto
{
    public long ContactCount { get; set; }
    public long CompanyCount { get; set; }
    public long OpenOpportunityCount { get; set; }
    public decimal OpenPipelineValue { get; set; }
}
