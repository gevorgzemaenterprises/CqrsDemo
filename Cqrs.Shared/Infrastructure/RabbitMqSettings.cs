using Cqrs.Shared.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cqrs.Shared.Infrastructure
{
    public class RabbitMqSettings : IRabbitMqSettings
    {
        public string HostName { get; set; } = "localhost";
        public string QueueName { get; set; } = "order_created_queue";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}
