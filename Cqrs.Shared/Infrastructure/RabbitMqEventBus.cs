using Cqrs.Shared.Interfaces;
using Cqrs.Shared.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

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

            _channel.QueueDeclare(queue: settings.QueueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += HandleReceivedEvent;

            _channel.BasicConsume(queue: settings.QueueName,
                                  autoAck: true,
                                  consumer: consumer);
        }

        public void Publish<T>(T @event) where T : class
        {
            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                  routingKey: _settings.QueueName,
                                  basicProperties: null,
                                  body: body);
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

            // try to determine event type from known handlers
            foreach (var handlerKey in _handlers.Keys)
            {
                try
                {
                    var type = Type.GetType($"Cqrs.Shared.Events.{handlerKey}, Cqrs.Shared");
                    if (type == null) continue;

                    var obj = JsonSerializer.Deserialize(message, type);
                    if (obj != null)
                    {
                        foreach (var handler in _handlers[handlerKey])
                            handler(obj);
                    }
                }
                catch { /* skip if deserialization fails */ }
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}

