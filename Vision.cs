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

        public void FindPlayers(ref int[] players) {
            if (!(players[1] >= 1 && players[1] <= 26) || !(players[0] >= 1 && players[0] <= 26)) {
                Console.WriteLine("Finding players");
                Rect rect = new Rect();
                GetWindowRect(_handle, ref rect);

                int width = rect.right - rect.left;
                Image<Bgr, Byte> screenshot = takeScreenshot(0, 0, width, 350);

                if (players[0] == 0 && FindPlayer(1, screenshot, ref players)) {
                    Console.WriteLine("Player 1 character found!");
                } else if (players[0] >= 1 && players[0] <= 26) {
                } else {
                    Console.WriteLine("Failed to find Player 1 character...");
                }
                if (players[1] == 0 && FindPlayer(2, screenshot, ref players)) {
                    Console.WriteLine("Player 2 character found!");
                } else if (players[1] >= 1 && players[1] <= 26) {
                } else {
                    Console.WriteLine("Failed to find Player 2 character...");
                }
            }
        }

        public bool FindPlayer(int player, Image<Bgr, Byte> source, ref int[] players) {
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
                Image<Bgr, Byte> template = new Image<Bgr, Byte>(image);
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

        public Image<Bgr, Byte> takeScreenshot(int left, int top, int right, int bottom) {
            Bitmap shot = ScreenCapture.CaptureWindow(_handle, left, top, right, bottom);
            Image<Bgr, Byte> screenshot = new Image<Bgr, Byte>(shot);

            //Show the image using ImageViewer from Emgu.CV.UI
            //ImageViewer.Show(screenshot, "Test Window");
            currentScreen = shot;
            return screenshot;
        }

        public Image<Bgr, Byte> takeScreenshot() {
            Bitmap shot = ScreenCapture.CaptureWindow(_handle);
            Image<Bgr, Byte> screenshot = new Image<Bgr, Byte>(shot);

            //Show the image using ImageViewer from Emgu.CV.UI
            ImageViewer.Show(screenshot, "Test Window");
            currentScreen = shot;
            return screenshot;
        }
    }
}
