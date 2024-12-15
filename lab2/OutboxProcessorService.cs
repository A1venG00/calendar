using lab2.Persistance;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Text;

namespace lab2
{
    public class OutboxProcessorService : BackgroundService
    {
        private readonly ApplicationDbContext _context;
        private readonly IModel _channel;

        public OutboxProcessorService(ApplicationDbContext context, IConnection connection)
        {
            _context = context;

            _channel = connection.CreateModel();
            _channel.QueueDeclare("notifications", durable: true, exclusive: false, autoDelete: false);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var messages = await _context.OutboxMessages
                    .Where(m => !m.IsSent)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    var body = Encoding.UTF8.GetBytes(message.MessageBody);
                    _channel.BasicPublish(exchange: "",
                        routingKey: "notifications",
                        basicProperties: null,
                        body: body);

                    message.IsSent = true;
                    message.SentAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}

