using RabbitMQ.Client;
using System.Text;

namespace lab2
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqPublisher()
        {
            var factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "notifications", durable: true, exclusive: false, autoDelete: false);
        }

        public virtual void PublishMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: "notifications", body: body);
        }
    }
}

