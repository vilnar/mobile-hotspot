using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.NetworkOperators;

namespace MobileHotspot
{
    class Program
    {
        private static bool isclosing = false;
        private static CancellationTokenSource cancellationTokenSource;
        private static Task hotspotTask;


        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(SetConsoleCtrlEventHandler handler, bool add);

        private delegate bool SetConsoleCtrlEventHandler(CtrlType sig);

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        static async Task Main(string[] args)
        {
            var root = Directory.GetCurrentDirectory();
            var isOk = DotEnv.Load(Path.Combine(root, ".env"));
            if (!isOk) {
                Console.ReadLine();
                return;
            }

            string ssid = Environment.GetEnvironmentVariable("SSID");
            if (string.IsNullOrEmpty(ssid) || ssid.Length < 2)
            {
                Console.Error.WriteLine("wrong env SSID");
                Console.ReadLine();
                return;
            }
            string passphrase = Environment.GetEnvironmentVariable("PASSPHRASE");
            if (string.IsNullOrEmpty(passphrase) || passphrase.Length < 2)
            {
                Console.Error.WriteLine("wrong env PASSPHRASE");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("wifi: " + ssid);
            Console.WriteLine("password: " + passphrase + "\n\n");

            var hotspot = await HotspotManager.CreateAsync(ssid, passphrase);

            hotspot.ClientConnected += (_, clientInfo) =>
            {
                Console.WriteLine($"CON | {ToString(clientInfo)}");
            };
            hotspot.ClientDisconnected += (_, clientInfo) =>
            {
                Console.WriteLine($"DIS | {ToString(clientInfo)}");
            };

            Console.WriteLine("Starting hotspot ... press \"ctrl+c\" to exit");

            cancellationTokenSource = new CancellationTokenSource();
            hotspotTask = hotspot.RunAsync(cancellationTokenSource.Token);
            SetConsoleCtrlHandler(Handler, true);

            while (!isclosing) ;
        }

        private static string ToString(NetworkOperatorTetheringClient client)
        {
            string hostNames = string.Join(" | ", client.HostNames);
            return $"{client.MacAddress} | {hostNames}";
        }

        private static bool Handler(CtrlType signal)
        {
            switch (signal)
            {
                case CtrlType.CTRL_BREAK_EVENT:
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    Console.WriteLine("Stopping hotspot ...");
                    cancellationTokenSource.Cancel();
                    hotspotTask.GetAwaiter().GetResult();
                    isclosing = true;
                    return false;

                default:
                    return false;
            }
        }
    }
}
