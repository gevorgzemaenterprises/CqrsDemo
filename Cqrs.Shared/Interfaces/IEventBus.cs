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
        void Subscribe<T>(Action<T> handler) where T : class;
    }
}
