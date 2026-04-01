namespace MyApp.Shared.Security;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string WarehouseStaff = "WarehouseStaff";
    public const string Viewer = "Viewer";

    public const string AdminOrWarehouseStaff = $"{Admin},{WarehouseStaff}";
    public const string AnyInventoryUser = $"{Admin},{WarehouseStaff},{Viewer}";

    public static readonly string[] All = [Admin, WarehouseStaff, Viewer];
}
