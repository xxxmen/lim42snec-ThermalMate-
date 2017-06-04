using System.Collections.Generic;
using System.IO;
using System.Reflection;


namespace ThermalMate
{
    static class Utils
    {
        public static void StreamToFile(Stream stream, string filePath, bool isOverlay)
        {
            // 将流读取到字节数组
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            if (File.Exists(filePath) && !isOverlay)
            {
                return;
            }
            var fs = new FileStream(filePath, FileMode.CreateNew);
            var bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close();

        }

        public static void ReleaseResource(string resourceName, string filePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            // 辅助方法：获取资源在程序集内部的名称
            //foreach (var file in assembly.GetManifestResourceNames())
            //  MessageBox.Show(file);
            var stream = assembly.GetManifestResourceStream(resourceName);
            StreamToFile(stream, filePath, false);

        }

        

        // 线性插值法
        public static double LineInterpolationModulus(List<Point> linePoints, double x)
        {
            int? index1 = null;
            for (var n = 0; n < linePoints.Count; n++)
            {
                var linePoint = linePoints[n];
                if (linePoint.X >= x)
                {
                    index1 = n;
                    break;
                }
            }

            int? index2 = null;
            for (var n = linePoints.Count - 1; n >= 0; n--)
            {
                var linePoint = linePoints[n];
                if (linePoint.X <= x)
                {
                    index2 = n;
                    break;
                }
            }

            if (!index1.HasValue || !index2.HasValue)
            {
                return -1;
            }

            var p1 = linePoints[index1.Value];
            var p2 = linePoints[index2.Value];
            if (index1.Value == index2.Value || index1 == index2)
            {
                return p1.Y;
            }

            var xDiff = p1.X - p2.X;
            var yDiff = p1.Y - p2.Y;
            var y = p1.Y + (x - p1.X) * yDiff / xDiff;

            return y;
        }

        public static List<Point> Modulus20 = new List<Point>()
        {
                new Point {X = 20, Y = 198},
                new Point {X = 100, Y = 183},
                new Point {X = 200, Y = 175},
                new Point {X = 250, Y = 171},
                new Point {X = 260, Y = 170},
                new Point {X = 280, Y = 168},
                new Point {X = 300, Y = 166},
                new Point {X = 320, Y = 165},
                new Point {X = 340, Y = 163},
                new Point {X = 350, Y = 162},
                new Point {X = 360, Y = 161}
        };

        public static List<Point> Modulus235 = new List<Point>()
        {
                new Point {X = 20, Y = 206},
                new Point {X = 100, Y = 200},
                new Point {X = 200, Y = 192},
                new Point {X = 250, Y = 188},
                new Point {X = 260, Y = 187},
                new Point {X = 280, Y = 186},
                new Point {X = 300, Y = 184},
        };

        public static List<Point> Modulus06Cr = new List<Point>()
        {
                new Point {X = 20, Y = 195},
                new Point {X = 100, Y = 191},
                new Point {X = 200, Y = 184},
                new Point {X = 250, Y = 181},
        };
        

        public static List<Point> Modulus12Cr = new List<Point>()
        {
                new Point {X = 20, Y = 208},
                new Point {X = 100, Y = 205},
                new Point {X = 200, Y = 201},
                new Point {X = 250, Y = 197},
                new Point {X = 260, Y = 196},
                new Point {X = 280, Y = 194},
                new Point {X = 300, Y = 192},
                new Point {X = 320, Y = 190},
                new Point {X = 340, Y = 188},
                new Point {X = 350, Y = 187},
                new Point {X = 360, Y = 186},

                new Point {X = 380, Y = 183},
                new Point {X = 400, Y = 181},
                new Point {X = 410, Y = 180},
                new Point {X = 420, Y = 178},
                new Point {X = 430, Y = 177},
                new Point {X = 440, Y = 175},
                new Point {X = 450, Y = 174},
                new Point {X = 460, Y = 172},
                new Point {X = 470, Y = 170},
                new Point {X = 480, Y = 168},
                new Point {X = 490, Y = 166},

                new Point {X = 500, Y = 165},
                new Point {X = 510, Y = 163},
                new Point {X = 520, Y = 162},
                new Point {X = 530, Y = 160},
                new Point {X = 540, Y = 158},
                new Point {X = 550, Y = 157},
                new Point {X = 560, Y = 153},
                new Point {X = 570, Y = 153},
                new Point {X = 580, Y = 152},

        };

        public static List<Point> Modulus15Cr = new List<Point>()
        {
                new Point {X = 20, Y = 206},
                new Point {X = 100, Y = 199},
                new Point {X = 200, Y = 190},
                new Point {X = 250, Y = 187},
                new Point {X = 260, Y = 186},
                new Point {X = 280, Y = 183},
                new Point {X = 300, Y = 181},
                new Point {X = 320, Y = 179},
                new Point {X = 340, Y = 177},
                new Point {X = 350, Y = 176},
                new Point {X = 360, Y = 175},

                new Point {X = 380, Y = 173},
                new Point {X = 400, Y = 172},
                new Point {X = 410, Y = 171},
                new Point {X = 420, Y = 170},
                new Point {X = 430, Y = 169},
                new Point {X = 440, Y = 168},
                new Point {X = 450, Y = 167},
                new Point {X = 460, Y = 166},
                new Point {X = 470, Y = 165},
                new Point {X = 480, Y = 164},
                new Point {X = 490, Y = 164},

                new Point {X = 500, Y = 163},
                new Point {X = 510, Y = 162},
                new Point {X = 520, Y = 161},
                new Point {X = 530, Y = 160},
                new Point {X = 540, Y = 159},
        };

        public class Point
        {
            public float X { get; set; }
            public float Y { get; set; }
        }
    }
}
