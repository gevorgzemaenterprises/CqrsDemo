using Cqrs.Shared.Interfaces;
using Cqrs.Shared.Settings;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Cqrs.Shared.Infrastructure
{

    public class RabbitMqEventBus : IEventBus, IDisposable
    {
        private readonly IRabbitMqSettings _settings;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        // Используем Func<T, Task> для асинхронной обработки
        private readonly Dictionary<string, List<Func<object, Task>>> _handlers = new();

        public RabbitMqEventBus(IRabbitMqSettings settings)
        {
            _settings = settings;

            var factory = new ConnectionFactory
            {
                HostName = settings.HostName,
                UserName = settings.UserName,
                Password = settings.Password,
                DispatchConsumersAsync = true // ✅ для поддержки async consumers
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("cqrs_exchange", ExchangeType.Direct, durable: true, autoDelete: false);
            _channel.QueueDeclare(settings.QueueName, durable: false, exclusive: false, autoDelete: false);
            _channel.QueueBind(settings.QueueName, "cqrs_exchange", routingKey: "OrderCreatedEvent");

            _channel.BasicQos(0, 1, false); // ✅ prefetch = 1 — не больше одного сообщения за раз

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += HandleReceivedEventAsync;

            _channel.BasicConsume(queue: settings.QueueName, autoAck: false, consumer: consumer);
        }

        public void Publish<T>(T @event) where T : class
        {
            var json = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(json);
            var properties = _channel.CreateBasicProperties();
            properties.DeliveryMode = 2;

            _channel.BasicPublish(
                exchange: "cqrs_exchange",
                routingKey: typeof(T).Name,
                basicProperties: properties,
                body: body
            );
        }

        public Task PublishAsync<T>(T @event) where T : class
        {
            Publish(@event);
            return Task.CompletedTask;
        }

        public void Subscribe<T>(Func<T, Task> handler) where T : class
        {
            var key = typeof(T).Name;
            if (!_handlers.ContainsKey(key))
                _handlers[key] = new List<Func<object, Task>>();

            _handlers[key].Add(async obj => await handler((T)obj));
        }

        private async Task HandleReceivedEventAsync(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var processed = false;

            foreach (var handlerKey in _handlers.Keys)
            {
                try
                {
                    var type = Type.GetType($"Cqrs.Shared.Events.{handlerKey}, Cqrs.Shared");
                    if (type == null) continue;

                    var obj = JsonConvert.DeserializeObject(message, type);
                    if (obj != null)
                    {
                        foreach (var handler in _handlers[handlerKey])
                        {
                            await handler(obj); // ✅ ожидаем завершения
                        }

                        processed = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RabbitMq] Error processing {handlerKey}: {ex.Message}");
                }
            }

            if (processed)
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            else
                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}

