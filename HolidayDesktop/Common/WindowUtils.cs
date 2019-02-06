using System;
using static HolidayDesktop.Common.NativeConstants;
using static HolidayDesktop.Common.NativeMethods;


namespace HolidayDesktop.Common
{
    static class WindowUtils
    {
        /*public static void ShowAlwaysOnDesktop(IntPtr hwnd)
        {
            var progmanHandle = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Progman", null);
            var shellHandle = FindWindowEx(progmanHandle, IntPtr.Zero, "SHELLDLL_DefView", null);
            if (shellHandle == IntPtr.Zero)
            {
                // Send 0x052C to Progman. This message directs Progman to spawn a 
                // WorkerW behind the desktop icons. If it is already there, nothing 
                // happens.
                SendMessageTimeout(progmanHandle, 0x052C, new IntPtr(0), IntPtr.Zero, SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out var result);
                var desktopHandle = GetDesktopWindow();
                var workerWHandle = IntPtr.Zero;
                do
                {
                    workerWHandle = FindWindowEx(desktopHandle, workerWHandle, "WorkerW", null);
                    shellHandle = FindWindowEx(workerWHandle, IntPtr.Zero, "SHELLDLL_DefView", null);
                } while (shellHandle == IntPtr.Zero && workerWHandle != IntPtr.Zero);
            }
            SetParent(hwnd, shellHandle);
        }*/

        public static void ShowAlwaysBehindDesktop(IntPtr hwnd)
        {
            var progmanHandle = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Progman", null);
            // Send 0x052C to Progman. This message directs Progman to spawn a 
            // WorkerW behind the desktop icons. If it is already there, nothing 
            // happens.
            SendMessageTimeout(progmanHandle, 0x052C, new IntPtr(0), IntPtr.Zero, SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out var result);
            var workerWHandle = IntPtr.Zero;
            // We enumerate all Windows, until we find one, that has the SHELLDLL_DefView 
            // as a child. 
            // If we found that window, we take its next sibling and assign it to workerw.
            EnumWindows(new EnumWindowsProc((topHandle, topParamHandle) =>
            {
                IntPtr shellHandle = FindWindowEx(topHandle, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shellHandle != IntPtr.Zero)
                {
                    // Gets the WorkerW Window after the current one.
                    workerWHandle = FindWindowEx(IntPtr.Zero, topHandle, "WorkerW", null);
                }

                return true;
            }), IntPtr.Zero);
            SetParent(hwnd, workerWHandle);
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