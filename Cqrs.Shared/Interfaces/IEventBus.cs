using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cqrs.Shared.Interfaces
{
    public interface IEventBus
    {
        void Publish<T>(T @event) where T : class;
        void Subscribe<T>(Func<T, Task> handler) where T : class; // ✅ async-safe handler
        Task PublishAsync<T>(T @event) where T : class;
    }
}
