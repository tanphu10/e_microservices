using Contracts.Common.Interfaces;
using Contracts.Messages;
using RabbitMQ.Client;
using System.Text;

namespace Infrastructure.Messages
{
    public class RabbitMQProducer : IMessageProducer

    {
        private readonly ISerializeService _serializeService;
        public RabbitMQProducer(ISerializeService serializeService)
        {
            _serializeService = serializeService;
        }
        public void SendMessage<T>(T message)
        {
            var connectionFatory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            var connection = connectionFatory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare("orders", exclusive: false);

            var jsonData = _serializeService.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonData);
            channel.BasicPublish(exchange: "", routingKey: "orders", body: body);

        }
    }
}
