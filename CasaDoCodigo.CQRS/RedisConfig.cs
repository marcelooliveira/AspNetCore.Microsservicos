using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC
{
    public class RedisConfig
    {
        public string ConnectionString { get; set; }
        public string EventBusConnection { get; set; }
    }
}
