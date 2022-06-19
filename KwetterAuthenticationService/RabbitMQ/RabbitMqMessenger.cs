using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KwetterAuthenticationService.RabbitMQ
{
    public class RabbitMqMessenger
    {
        public void SendRabbitMessage(string msg)
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq-clusterip-srv", Port = 5672 };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = msg;
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                         routingKey: "task_queue",
                                         basicProperties: properties,
                                         body: body);

                //channel.BasicPublish(exchange: "",
                //                     routingKey: "task_queue",
                //                     basicProperties: null,
                //                     body: body);

                channel.Close();
            }
        }
    }
}
