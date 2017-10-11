using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPC.Common.Registry
{
    public class ConsulRegistryService : BaseConsulService,IRegistryService
    {
        public ConsulRegistryService(ServiceRegistryAddress registryAddress) : base(registryAddress)
        {
        }

        public void Deregister(RpcService rpcService)
        {
            using (var consul = BuildConsul())
            {
                var result = consul.Agent.ServiceDeregister(GetServiceId(rpcService)).ConfigureAwait(false).GetAwaiter().GetResult();
            };
        }

        public void Register(RpcService rpcService)
        {
            using (var consul = BuildConsul())
            {
                var serviceRegistration = new AgentServiceRegistration()
                {
                    ID = GetServiceId(rpcService),
                    Name = rpcService.Name,
                    Address = rpcService.Host,
                    Port = rpcService.Port,
                    Check= new AgentServiceCheck
                    {
                         HTTP=$"http://{rpcService.Host}:{rpcService.Port}",
                         Interval=TimeSpan.FromSeconds(5),
                         Timeout= TimeSpan.FromSeconds(2)
                    }
                };
                var result = consul.Agent.ServiceRegister(serviceRegistration).ConfigureAwait(false).GetAwaiter().GetResult();
            };
  
        }
        private string GetServiceId(RpcService rpcService)
        {
            return $"{rpcService.Name}_{rpcService.Host}_{rpcService.Port}";
        }
    }
}
