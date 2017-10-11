using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPC.Common.Discovery
{
    public class ConsulDiscoveryService : BaseConsulService, IDiscoveryService
    {
        public ConsulDiscoveryService(ServiceRegistryAddress registryAddress) : base(registryAddress)
        {
        }

        public List<RpcService> GetRpcService(string serviceName)
        {
            using (var consul = BuildConsul())
            {
                var healthServices = consul.Health.Service(serviceName,"",true).ConfigureAwait(false).GetAwaiter().GetResult();//获取健康的服务
                //var services=consul.Agent.Services().ConfigureAwait(false).GetAwaiter().GetResult();
                //var discoveredServices = services.Response.Values.Where(t=>t.Service.Equals(serviceName));
                var discoveredServices = healthServices.Response.Select(t => t.Service).ToList();
                return discoveredServices?.Select(t => new RpcService { Name = t.Service, Host = t.Address, Port = t.Port }).ToList();
            };
        }
    }
}
