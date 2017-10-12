using Consul;
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

        public string GetRpcService(string serviceName)
        {
            using (var consul = BuildConsul())
            {
                //获取健康的Traefik服务
                var traefikServices = consul.Health.Service(Global.TraefikServiceName, "", true).ConfigureAwait(false).GetAwaiter().GetResult().Response.Select(t => t.Service).ToList();
                var traefikService = GetRandomService(traefikServices);
                //如果使用Traefik作为consul上服务的负载均衡
                if (traefikService != null)
                {
                    return $"http://{traefikService.Address}:{traefikService.Port}/{serviceName}";
                }
                //如果没有就直接随机Consul中取出的健康服务
                var discoveredServices = consul.Health.Service(serviceName,"",true).ConfigureAwait(false).GetAwaiter().GetResult().Response.Select(t => t.Service).ToList();//获取健康的服务
                var discoveredService = GetRandomService(discoveredServices);
                return $"http://{discoveredService.Address}:{discoveredService.Port}";
            };
        }


        /// <summary>
        /// 获取随机的服务
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private AgentService GetRandomService(IList<AgentService> list)
        {
            if (list == null || !list.Any())
            {
                return null;
            }
            Random rnd = new Random();
            return list.ElementAt(rnd.Next(list.Count));
        }
    }
}
