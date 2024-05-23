
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;



var connectionFactory = new ConnectionFactory
{
    HostName = "localhost"
};


var connection = connectionFactory.CreateConnection();
using var channel = connection.CreateModel();
channel.QueueDeclare("orders", exclusive: false);
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (_, EventArgs) =>
{
    var body = EventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"message received : {message}");
};

channel.BasicConsume(queue: "orders", autoAck: true, consumer: consumer);
Console.ReadKey();