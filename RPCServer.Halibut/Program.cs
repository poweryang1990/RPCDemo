using Halibut;
using Halibut.ServiceModel;
using IService;
using Serilog;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RPCServer
{
    class Program
    {
        const string SslCertificateThumbprint = "6E5C6492129B75A4C83E1A23797AF6344092E5C2";
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .MinimumLevel.Warning()
                .CreateLogger();
            Console.Title = "Halibut Server";
            var certificate = new X509Certificate2("HalibutServer.pfx");

            var endPoint = new IPEndPoint(IPAddress.IPv6Any, 8433);

            var services = new DelegateServiceFactory();
            services.Register<IUserService>(() => new UserService());

            using(var server =new HalibutRuntime(services, certificate))
            {
                server.Poll(new Uri("poll://SQ-TENTAPOLL"), new ServiceEndPoint(new Uri("wss://localhost:8433/Halibut"), SslCertificateThumbprint));
                Console.WriteLine("Server listening on port 8433. Type 'exit' to quit, or 'cls' to clear...");
                while (true)
                {
                    var line = Console.ReadLine();
                    if (string.Equals("cls", line, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Clear();
                        continue;
                    }

                    if (string.Equals("q", line, StringComparison.OrdinalIgnoreCase))
                        return;

                    if (string.Equals("exit", line, StringComparison.OrdinalIgnoreCase))
                        return;

                    Console.WriteLine("Unknown command. Enter 'q' to quit.");
                }
            }
        }
    }
}
