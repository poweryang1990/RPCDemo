using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPC.Common
{
    public abstract class BaseConsulService
    {
        private readonly ServiceRegistryAddress _registryAddress;
        public BaseConsulService(ServiceRegistryAddress registryAddress)
        {
            _registryAddress = registryAddress;
        }
        protected ConsulClient BuildConsul()
        {
            var config = new ConsulClientConfiguration();
            config.Address = new Uri($"http://{_registryAddress.RegistryHost}:{_registryAddress.RegistryPort}");
#pragma warning disable CS0618 // 类型或成员已过时
            return new ConsulClient(config);
#pragma warning restore CS0618 // 类型或成员已过时
        }
    }
}
