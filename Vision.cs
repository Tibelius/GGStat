using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.Windows.Forms;
using System.Resources;

namespace GGStat
{
    class Vision
    {
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public Bitmap currentScreen;
        private IntPtr _handle;

        Dictionary<string, string> static_templates;

        [DllImport("user32.dll")]
        private static extern IntPtr ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        public Vision(IntPtr handle) {
            _handle = handle;
        }
        
        public bool FindPlayers(ref int[] players) {
            if (Program.DEBUG) Console.WriteLine("Finding players");
            Rect rect = new Rect();
            GetWindowRect(_handle, ref rect);

            int width = rect.right - rect.left;
            Image<Bgr, byte> screenshot = takeScreenshot(0, 0, width, 350);

            if (players[0] == 0 && FindPlayer(1, screenshot, ref players)) {
                if (Program.DEBUG) Console.WriteLine("Player 1 character found!");
            } else if (players[0] >= 1 && players[0] <= 26) {
            } else {
                if (Program.DEBUG) Console.WriteLine("Failed to find Player 1 character...");
            }
            if (players[1] == 0 && FindPlayer(2, screenshot, ref players)) {
                if (Program.DEBUG) Console.WriteLine("Player 2 character found!");
            } else if (players[1] >= 1 && players[1] <= 26) {
            } else {
                if (Program.DEBUG) Console.WriteLine("Failed to find Player 2 character...");
            }

            return players[0] != 0 && players[1] != 0;
        }

        public bool FindPlayer(int player, Image<Bgr, byte> source, ref int[] players) {
            if (Program.DEBUG) Console.WriteLine("Finding Player " + player + " character...");
            string prefix;
            if (player == 1) {
                prefix = "p1_";
            } else {
                prefix = "p2_";
            }

            var a = Assembly.GetExecutingAssembly();
            bool success = false;
            for (int i = 1; i < Game.Character.Length; i++) {
                Bitmap image = new Bitmap(a.GetManifestResourceStream("GGStat.Properties."+ prefix + Game.Character[i] + ".png"));
                Image<Bgr, byte> template = new Image<Bgr, byte>(image);
                SearchOne(source, template, 0.7, new Rectangle(0, 0, source.Width, source.Height), out success);
                if(success) {
                    players[player - 1] = i;
                    break;
                }
            }
            return success;
        }
        
        public static Point SearchOne(Image<Bgr, byte> srcImg, Image<Bgr, byte> smpImg, double treshold, Rectangle roi, out bool success) {
            srcImg.ROI = roi;
            Image<Gray, float> matched = srcImg.MatchTemplate(smpImg, TemplateMatchingType.CcoeffNormed);

            matched.MinMax(out double[] minValues, out double[] maxValues, out Point[] minLocations, out Point[] maxLocations);
            matched.Dispose();

            if (maxValues[0] > treshold) {
                success = true;
                int x = roi.X + maxLocations[0].X;
                int y = roi.Y + maxLocations[0].Y;
                return new Point(x, y);
            }
            success = false;
            return Point.Empty;
        }

        public Image<Bgr, byte> takeScreenshot(int left, int top, int right, int bottom) {
            Bitmap shot = ScreenCapture.CaptureWindow(_handle, left, top, right, bottom);
            Image<Bgr, byte> screenshot = new Image<Bgr, byte>(shot);

            //Show the image using ImageViewer from Emgu.CV.UI
            //ImageViewer.Show(screenshot, "Test Window");
            currentScreen = shot;
            return screenshot;
        }

        public Image<Bgr, byte> takeScreenshot() {
            Bitmap shot = ScreenCapture.CaptureWindow(_handle);
            Image<Bgr, byte> screenshot = new Image<Bgr, byte>(shot);

            //Show the image using ImageViewer from Emgu.CV.UI
            ImageViewer.Show(screenshot, "Test Window");
            currentScreen = shot;
            return screenshot;
        }
    }
}
