using System;
using static HolidayDesktop.Common.NativeConstants;
using static HolidayDesktop.Common.NativeMethods;


namespace HolidayDesktop.Common
{
    static class WindowUtils
    {
        public static void ShowAlwaysOnDesktop(IntPtr hwnd)
        {
            var progmanHandle = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Progman", null);
            var shellHandle = FindWindowEx(progmanHandle, IntPtr.Zero, "SHELLDLL_DefView", null);
            if (shellHandle == IntPtr.Zero)
            {
                var desktopHandle = GetDesktopWindow();
                var workerWHandle = IntPtr.Zero;
                do
                {
                    workerWHandle = FindWindowEx(desktopHandle, workerWHandle, "WorkerW", null);
                    shellHandle = FindWindowEx(workerWHandle, IntPtr.Zero, "SHELLDLL_DefView", null);
                } while (shellHandle == IntPtr.Zero && workerWHandle != IntPtr.Zero);
            }
            SetParent(hwnd, shellHandle);
        }

        public static void SetStyles(IntPtr hwnd)
        {
            var exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            exStyle |= WS_EX_TOOLWINDOW;
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);
            SetWindowPos(hwnd, new IntPtr(HWND_BOTTOM), 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);
        }

        public static void EnableNoActive(IntPtr hwnd, bool enable)
        {
            var exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            if (enable)
            {
                exStyle |= WS_EX_NOACTIVATE;
            }
            else
            {
                exStyle &= ~WS_EX_NOACTIVATE;
            }
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);
        }
    }
}
