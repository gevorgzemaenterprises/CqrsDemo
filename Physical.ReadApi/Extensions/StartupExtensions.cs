using Cqrs.Shared.Events;
using Cqrs.Shared.Interfaces;
using Physical.ReadApi.EventHandlers;
using Cqrs.Shared.Extensions;

namespace Physical.ReadApi.Extensions
{
    public static class StartupExtensions
    {
        public static void UseEventBusSubscriptions(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            var eventBus = serviceProvider.GetRequiredService<IEventBus>();

            // 👇 Подписываемся через расширение
            eventBus.SubscribeScoped<OrderCreatedEvent, OrderCreatedEventHandler>(serviceProvider);
        }
    }
}
