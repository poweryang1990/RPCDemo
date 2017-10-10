using Hprose.IO;
using Hprose.Server;
using IService;
using Model;
using RPC.Common;
using RPC.Common.Registry;
using Service;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCServer.Hprose
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
            container.Register(()=>new ServiceRegistryAddress { RegistryHost = "127.0.0.1", RegistryPort = 8500 }, Lifestyle.Scoped);
            container.Register<IUserService, UserService>(Lifestyle.Scoped);
            container.Register<IRegistryService, ConsulRegistryService>(Lifestyle.Scoped);
            // Optionally verify the container.
            container.Verify();
        }
        static void Main(string[] args)
        {
            //HproseClassManager.Register(typeof(User), "User");
            HproseHttpListenerServer server = new HproseHttpListenerServer("http://127.0.0.1:2012/");
            using (ThreadScopedLifestyle.BeginScope(container))
            {
                var registryService = container.GetInstance<IRegistryService>();
                registryService.Register(new RPC.Common.RpcService { Name="房源服务", Host = "127.0.0.1", Port = 2012 });
                server.RegisterService<IUserService>(container.GetInstance<IUserService>());

               
            }
            server.IsCrossDomainEnabled = true;
            
            //server.CrossDomainXmlFile = "crossdomain.xml";
            server.Start();
            Console.WriteLine("Server started.");
            Console.ReadLine();
            Console.WriteLine("Server stopped.");
        }

         
    }

    public static class HproseServiceExtentions
    {
        /// <summary>
        /// 注册所有公共方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="server"></param>
        /// <param name="implement"></param>
        public static void RegisterService<T>(this HproseService server, object implement) where T :class
        {
            var methods = typeof(T).GetMethods().Where(t=>t.IsPublic).Select(t => t.Name).ToArray();
            server.Add(methods, implement);
        }
    }
}
