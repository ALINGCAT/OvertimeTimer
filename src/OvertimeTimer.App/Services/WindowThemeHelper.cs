using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace OvertimeTimer.App.Services;

public static class WindowThemeHelper
{
    private const int DwmwaUseImmersiveDarkMode = 20;
    private const int DwmwaUseImmersiveDarkModeBefore20H1 = 19;

    public static bool IsDark { get; private set; }

    public static event Action? ThemeChanged;

    public static void SetDark(bool isDark)
    {
        if (IsDark == isDark)
        {
            return;
        }

        IsDark = isDark;
        ThemeChanged?.Invoke();
    }

    public static void Attach(Window window)
    {
        void Apply() => ApplyDarkTitleBar(window, IsDark);

        window.SourceInitialized += (_, _) => Apply();
        ThemeChanged += Apply;
        window.Closed += (_, _) => ThemeChanged -= Apply;
    }

    private static void ApplyDarkTitleBar(Window window, bool isDark)
    {
        try
        {
            var handle = new WindowInteropHelper(window).Handle;
            if (handle == IntPtr.Zero)
            {
                return;
            }

            var value = isDark ? 1 : 0;
            if (DwmSetWindowAttribute(handle, DwmwaUseImmersiveDarkMode, ref value, sizeof(int)) != 0)
            {
                DwmSetWindowAttribute(handle, DwmwaUseImmersiveDarkModeBefore20H1, ref value, sizeof(int));
            }
        }
        catch
        {
            // Older Windows versions do not support the immersive dark title bar.
        }
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);
}
