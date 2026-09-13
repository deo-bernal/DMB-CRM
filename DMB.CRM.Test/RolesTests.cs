using Dmb.Crm.Model;
using Xunit;

namespace Dmb.Crm.Test;

public class RolesTests
{
    [Fact]
    public void Location_roles_match_ghl_sub_account_roles()
    {
        Assert.Contains(Roles.Owner, Roles.All);
        Assert.Contains(Roles.Admin, Roles.All);
        Assert.Contains(Roles.User, Roles.All);
        Assert.Equal(3, Roles.All.Length);
    }
}
