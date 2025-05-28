using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cqrs.Shared.Settings
{
    public interface IRabbitMqSettings
    {
        string HostName { get; }
        string QueueName { get; }
        string UserName { get; }
        string Password { get; }
    }
}
