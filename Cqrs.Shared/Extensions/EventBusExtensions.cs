using Cqrs.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Cqrs.Shared.Extensions
{
    public static class EventBusExtensions
    {
        public static void SubscribeScoped<TEvent, THandler>(this IEventBus bus, IServiceProvider provider)
            where TEvent : class
            where THandler : IIntegrationEventHandler<TEvent>
        {
            bus.Subscribe<TEvent>(async evt =>
            {
                using var scope = provider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<THandler>();
                await handler.HandleAsync(evt);
            });
        }
    }
}
