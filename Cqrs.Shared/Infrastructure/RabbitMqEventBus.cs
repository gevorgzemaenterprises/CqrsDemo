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

        private readonly Dictionary<string, List<Action<object>>> _handlers = new();

        public RabbitMqEventBus(IRabbitMqSettings settings)
        {
            _settings = settings;

            var factory = new ConnectionFactory()
            {
                HostName = settings.HostName,
                UserName = settings.UserName,
                Password = settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // 👇️ ОБЯЗАТЕЛЬНО СОЗДАЕМ EXCHANGE ПЕРЕД ПУБЛИКАЦИЕЙ
            _channel.ExchangeDeclare(
                exchange: "cqrs_exchange",
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                arguments: null);

            _channel.QueueDeclare(
                queue: settings.QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // ❗ Если ты хочешь, чтобы очередь получала события с exchange — нужно биндинг сделать:
            // (можно сделать универсально позже, сейчас хотя бы для OrderCreatedEvent)
            _channel.QueueBind(
                queue: settings.QueueName,
                exchange: "cqrs_exchange",
                routingKey: "OrderCreatedEvent");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += HandleReceivedEvent;

            _channel.BasicConsume(
                queue: settings.QueueName,
                autoAck: false,
                consumer: consumer);
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

        public void Subscribe<T>(Action<T> handler) where T : class
        {
            var key = typeof(T).Name;
            if (!_handlers.ContainsKey(key))
                _handlers[key] = new List<Action<object>>();

            _handlers[key].Add(e => handler((T)e));
        }

        private void HandleReceivedEvent(object? model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            bool processed = false;

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
                            handler(obj);

                        processed = true;
                        break; 
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RabbitMq] Failed to handle {handlerKey}: {ex.Message}");
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

