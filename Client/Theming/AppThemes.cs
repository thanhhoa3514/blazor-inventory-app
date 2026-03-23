using MudBlazor;

namespace MyApp.Client.Theming;

public static class AppThemes
{
    public static MudTheme CorporateTheme { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#1f4b99",
            Secondary = "#3f6ad8",
            Tertiary = "#0f766e",
            Info = "#0284c7",
            Success = "#15803d",
            Warning = "#b45309",
            Error = "#b42318",
            Background = "#f5f7fb",
            Surface = "#ffffff",
            AppbarBackground = "#ffffff",
            AppbarText = "#1f2937",
            DrawerBackground = "#0f172a",
            DrawerText = "#e5e7eb",
            DrawerIcon = "#cbd5e1",
            TextPrimary = "#111827",
            TextSecondary = "#4b5563",
            LinesDefault = "#dbe2ef",
            TableLines = "#e6ebf3"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px"
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[] { "Segoe UI", "Inter", "Roboto", "Arial", "sans-serif" }
            },
            H1 = new H1Typography
            {
                FontWeight = "700"
            },
            H2 = new H2Typography
            {
                FontWeight = "700"
            },
            H3 = new H3Typography
            {
                FontWeight = "700"
            }
        }
    };
}
