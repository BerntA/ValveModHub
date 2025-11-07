namespace ValveModHub.Desktop.Utils;

public static class ColorThemeHelper
{
    public static void SetSteamDefaultLayout(Control control)
    {
        control.BackColor = Color.FromArgb(255, 73, 78, 73);
        control.ForeColor = Color.White;
    }

    public static void SetSteamHeaderLayout(Control control)
    {
        control.BackColor = Color.FromArgb(255, 70, 70, 70);
        control.ForeColor = Color.White;
    }

    public static void SetTransparent(Control control)
    {
        control.BackColor = Color.Transparent;
        control.ForeColor = Color.White;
    }
}