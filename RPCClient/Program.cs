using Halibut;
using Hprose.Client;
using IService;
using Model;
using RPC.Common;
using RPC.Common.Discovery;
using Serilog;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace RPCClient
{
    class Program
    {
        private static Container container;
        static Program()
        {
            // Create the container as usual.
            container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
            // Register your types, for instance:
            #region Halibut
            //var certificate = new X509Certificate2("HalibutClient.pfx");

            //using (var runtime = new HalibutRuntime(certificate))
            //{

            //    //Begin make request of WebSocket Polling server
            //    AddSslCertToLocalStoreAndRegisterFor("0.0.0.0:8433");
            //    runtime.ListenWebSocket("https://+:8433/Halibut");
            //    runtime.Trust("EF3A7A69AFE0D13130370B44A228F5CD15C069BC");
            //    var userservice = runtime.CreateClient<IUserService>("poll://SQ-TENTAPOLL", "2074529C99D93D5955FEECA859AEAC6092741205");
            //    container.Register<IUserService>(() => userservice, Lifestyle.Scoped);
            //}
            #endregion
            #region Hprose
            using (ThreadScopedLifestyle.BeginScope(container))
            {
                var consulDiscoveryService = new ConsulDiscoveryService(new ServiceRegistryAddress { RegistryHost = "127.0.0.1", RegistryPort = 8500 });
                var urls = consulDiscoveryService.GetRpcService("UserService");
                if (urls != null && urls.Any())
                {
                    string url = $"http://{urls[0].Host}:{urls[0].Port}/";
                    container.Register<IUserService>(() => new HproseHttpClientPxoxy(url).UseService<IUserService>(), Lifestyle.Scoped);
                }
            }
            #endregion

            // Optionally verify the container.
            container.Verify();
        }
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo.ColoredConsole()
               .CreateLogger();

            Console.Title = "RPC  Client Test";
            while (true)
            {
                using (ThreadScopedLifestyle.BeginScope(container))
                {
                    IUserService userService = container.GetInstance<IUserService>();
                    var result = userService.SayHello(new User { name = "Power Yang", age = 19 });
                    var users = userService.GetAllUsers();
                    Console.WriteLine(result);
                }
                Console.ReadKey();
            }
        }

        static void AddSslCertToLocalStoreAndRegisterFor(string address)
        {
            var certificate = new X509Certificate2("HalibutSslCertificate.pfx", "password");
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            store.Add(certificate);
            store.Close();


            var proc = new Process()
            {
                StartInfo = new ProcessStartInfo("netsh", $"http add sslcert ipport={address} certhash={certificate.Thumbprint} appid={{2e282bfb-fce9-40fc-a594-2136043e1c8f}}")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            proc.Start();
            proc.WaitForExit();
            var output = proc.StandardOutput.ReadToEnd();

            if (proc.ExitCode != 0 && !output.Contains("Cannot create a file when that file already exists"))
            {
                Console.WriteLine(output);
                Console.WriteLine(proc.StandardError.ReadToEnd());
                throw new Exception("Could not bind cert to port");
            }
        }
    }

    public  class HproseHttpClientPxoxy
    {
        private  HproseHttpClient _client;

        public HproseHttpClientPxoxy(HproseHttpClient client)
        {
            _client = client;
        }
        public HproseHttpClientPxoxy(string url)
        {
            _client = new HproseHttpClient(url);
        }

        public T UseService<T>() => _client.UseService<T>();
    }
}
