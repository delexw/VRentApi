using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Configuration;

namespace DeductionScheduleJobListener
{
    class Program
    {
        private static int _port;
        private static byte[] message = new byte[1024];
        static bool _IsExit = false;
        static void Main(string[] args)
        {
            ConsoleWin32Helper.ShowNotifyIcon();
            ConsoleWin32Helper.DisableCloseButton(Console.Title);

            Socket host = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress hostIP;
            if (IPAddress.TryParse(ConfigurationManager.AppSettings["ip"], out hostIP))
            {
                _port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
                host.Bind(new IPEndPoint(hostIP, _port));
                host.Listen(0);
                Console.WriteLine("Start to listen debugger information from Schedule Job...");
                host.BeginAccept(new AsyncCallback(ClientAccepted), host);
                Thread threadMonitorInput = new Thread(new ThreadStart(MonitorInput));
                threadMonitorInput.Start();

                while (true)
                {
                    //Force to capture the win message
                    Application.DoEvents();
                    if (_IsExit)
                    {
                        host.Close();
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid IP address to socket binding");
                Console.ReadLine();
            }
        }
        static void MonitorInput()
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (input == "exit")
                {
                    _IsExit = true;
                    Thread.CurrentThread.Abort();
                }
            }
        }

        private static void ClientAccepted(IAsyncResult o)
        {
            try
            {
                Socket host = (Socket)o.AsyncState;
                var client = host.EndAccept(o);
                client.BeginReceive(message, 0, message.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);
                host.BeginAccept(new AsyncCallback(ClientAccepted), host);
            }
            catch (ObjectDisposedException ode)
            {
                Console.WriteLine("Socket is closed");
            }
        }

        private static void ReceiveMessage(IAsyncResult o)
        {
            try
            {
                Console.WriteLine(Encoding.UTF8.GetString(message).Replace("\0", ""));
                Socket client = o.AsyncState as Socket;
                client.EndReceive(o);
                if (client.Connected)
                {
                    client.Send(Encoding.UTF8.GetBytes("Ping"));
                    client.BeginReceive(message, 0, message.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);
                }
            }
            catch (ObjectDisposedException ode)
            {
                Console.WriteLine("Socket is closed");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Schedule Job is disconnected");
            }
            //message.ToArray().
        }
    }
    class ConsoleWin32Helper
    {
        static ConsoleWin32Helper()
        {
            _NotifyIcon.Icon = new Icon(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "red-upper-V.ico");
            _NotifyIcon.Visible = false;
            _NotifyIcon.Text = "VRent Schedule Job Listener";
            ContextMenu menu = new ContextMenu();

            menu.MenuItems.Add("Open", (o, v) => {
                Show();
            });
            menu.MenuItems.Add("Hide", (o, v) =>
            {
                Hidden();
            });
            _NotifyIcon.ContextMenu = menu;
           
        }

        #region
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        static extern IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);
        [DllImport("User32.dll", EntryPoint = "ShowWindow")]
        public static extern bool ShowWindow(IntPtr hwind, int cmdShow);
        [DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hwind);

        ///<summary>
        /// 
        ///</summary>
        ///<param name="consoleName"></param>
        public static void DisableCloseButton(string title)
        {
            Thread.Sleep(100);
            IntPtr windowHandle = FindWindow(null, title);
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 0xF060;
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
        }
        public static bool IsExistsConsole(string title)
        {
            IntPtr windowHandle = FindWindow(null, title);
            if (windowHandle.Equals(IntPtr.Zero))
            {
                return false;
            }
            return true;
        }

        public static void Hidden()
        {
            IntPtr ParenthWnd = new IntPtr(0);
            IntPtr et = new IntPtr(0);
            ParenthWnd = FindWindow(null, Console.Title);
            int normalState = 0;
            ShowWindow(ParenthWnd, normalState);
        }

        public static void Show()
        {
            IntPtr ParenthWnd = new IntPtr(0);
            IntPtr et = new IntPtr(0);
            ParenthWnd = FindWindow(null, Console.Title);
            int normalState = 9;
            ShowWindow(ParenthWnd, normalState);
        }

        #endregion
        #region
        static NotifyIcon _NotifyIcon = new NotifyIcon();
        public static void ShowNotifyIcon()
        {
            _NotifyIcon.Visible = true;
            _NotifyIcon.ShowBalloonTip(3000, "", "VRent Schedule Job Listener", ToolTipIcon.None);
        }
        public static void HideNotifyIcon()
        {
            _NotifyIcon.Visible = false;
        }
        #endregion
    }
}
