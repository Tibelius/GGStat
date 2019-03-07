using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace GGStat
{
    public class ScreenCapture
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDesktopWindow();

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        public static Bitmap CaptureDesktop() {
            return CaptureWindow(GetDesktopWindow());
        }

        public static Bitmap CaptureActiveWindow() {
            return CaptureWindow(GetForegroundWindow());
        }
        
        public static Bitmap CaptureWindow(IntPtr handle ) {
            Rect rect = new Rect();
            GetWindowRect(handle, ref rect);
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var graphics = Graphics.FromImage(result)) {
                graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }

            return result;
        }

        public static Bitmap CaptureWindow(IntPtr handle, int left, int top, int right, int bottom) {
            Rect rect = new Rect();
            GetWindowRect(handle, ref rect);
            rect.Left += left;
            rect.Top += top;
            rect.Right = rect.Left + right;
            rect.Bottom = rect.Top + bottom;

            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var graphics = Graphics.FromImage(result)) {
                graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }

            return result;
        }
    }
}
