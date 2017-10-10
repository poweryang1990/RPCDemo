using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPC.Common
{
    public class ServiceRegistryAddress
    {
        /// <summary>
        /// 服务注册地址
        /// </summary>
        public string RegistryHost { get; set; }
        /// <summary>
        /// 服务注册端口
        /// </summary>
        public int RegistryPort { get; set; }
    }
}
