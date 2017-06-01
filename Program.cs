using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ThermalMate
{
    internal static class Program
    {
        private const int WS_SHOWNORMAL = 1;

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var instance = GetRunningInstance();
            if (instance == null)
            {
                var title = string.Format("XNote Ver{0}   『 C0de by hangch 』", Application.ProductVersion);
                Application.Run(new ForMain { Text = title });
            }
            else
            {
                ShowRunningInstance(instance);
            }
        }

        public static Process GetRunningInstance()
        {
            var current = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(current.ProcessName);
            return processes.Where(process => process.Id != current.Id).FirstOrDefault(process => Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName);
        }

        public static void ShowRunningInstance(Process instance)
        {
            ShowWindowAsync(instance.MainWindowHandle, WS_SHOWNORMAL);
            SetForegroundWindow(instance.MainWindowHandle);
        }
    }
}
