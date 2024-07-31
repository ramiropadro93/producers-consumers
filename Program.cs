using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        var producerCount = int.Parse(Environment.GetEnvironmentVariable("PRODUCER_COUNT") ?? "2");
        var consumerCount = int.Parse(Environment.GetEnvironmentVariable("CONSUMER_COUNT") ?? "2");
        var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";

        var tasks = new Task[producerCount + consumerCount];

        for (int i = 0; i < producerCount; i++)
        {
            var producerNumber = i + 1;
            tasks[i] = Task.Run(() => Producer(rabbitMqHost, producerNumber));
        }

        for (int i = 0; i < consumerCount; i++)
        {
            var consumerNumber = i + 1;
            tasks[producerCount + i] = Task.Run(() => Consumer(rabbitMqHost, consumerNumber));
        }

        await Task.WhenAll(tasks);
    }

    static void Producer(string hostName, int producerNumber)
    {
        var factory = new ConnectionFactory() { HostName = hostName };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "producers_consumers_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        var cantidadMensajes = int.Parse(Environment.GetEnvironmentVariable("MESSAGE_COUNT") ?? "10");
        for (int i = 0; i < cantidadMensajes; i++)
        {
            string message = $"[M] Mensaje {i + 1} de Productor - " + producerNumber;
            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "", routingKey: "producers_consumers_queue", basicProperties: properties, body: body);
            Console.WriteLine($"[x] Productor {producerNumber} - Envía {message}");
            Thread.Sleep(1000);
        }
    }

    static void Consumer(string hostName, int consumerNumber)
    {
        var factory = new ConnectionFactory() { HostName = hostName };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: "producers_consumers_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, res) =>
        {
            var body = res.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"[x] Consumidor [{consumerNumber}] - Recibe {message}");
            channel.BasicAck(deliveryTag: res.DeliveryTag, multiple: false);
        };

        channel.BasicConsume(queue: "producers_consumers_queue", autoAck: false, consumer: consumer);
        Console.WriteLine($"[*] Consumidor {consumerNumber} esperando...");

        while (true)
        {
            Thread.Sleep(100);
        }
    }
}
