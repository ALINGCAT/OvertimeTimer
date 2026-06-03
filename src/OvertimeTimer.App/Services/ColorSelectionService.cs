namespace OvertimeTimer.App.Services;

public sealed class ColorSelectionService : IColorSelectionService
{
    public string? ChooseColor(string initialColor)
    {
        var color = TryParseColor(initialColor, out var parsedColor)
            ? parsedColor
            : System.Windows.Media.Colors.White;

        using var dialog = new System.Windows.Forms.ColorDialog
        {
            FullOpen = true,
            AllowFullOpen = true,
            AnyColor = true,
            Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)
        };

        return dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK
            ? $"#{dialog.Color.A:X2}{dialog.Color.R:X2}{dialog.Color.G:X2}{dialog.Color.B:X2}"
            : null;
    }

    private static bool TryParseColor(string colorText, out System.Windows.Media.Color color)
    {
        color = System.Windows.Media.Colors.White;
        try
        {
            color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorText);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
