using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPC.Common
{
    public class RpcService
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 服务地址
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 服务端口
        /// </summary>
        public int Port { get; set; }
        
    }
}
