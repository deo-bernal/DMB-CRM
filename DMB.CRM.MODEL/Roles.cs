namespace Dmb.Crm.Model;

public static class Roles
{
    public const string Owner = "owner";
    public const string Admin = "admin";
    public const string User = "user";

    public static readonly string[] All = [Owner, Admin, User];
}
