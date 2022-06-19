using DataAccesLayer;
using Logic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KwetterAuthenticationService.RabbitMQ
{
    public class RabbitMqSubscriber : BackgroundService
    {
        private IConnection connection;
        private IModel channel;
        private readonly AuthenticationLogic logic;
        private readonly IServiceScopeFactory scopeFactory;
        private string queuename = "";
        private bool isConnected = false;

        public RabbitMqSubscriber(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
            var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            var tokenBuilder = scope.ServiceProvider.GetRequiredService<ITokenBuilder>();
            
            logic = new AuthenticationLogic(dbContext, tokenBuilder);
            StartClient();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (isConnected)
            {
                stoppingToken.ThrowIfCancellationRequested();

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    //todo put own logic here
                    var id = Convert.ToInt32(message);
                    logic.DeleteCredentials(id);
                };
                channel.BasicConsume(queue: queuename,
                                     autoAck: true,
                                     consumer: consumer);
            }
            

            return Task.CompletedTask;
        }

        private void StartClient()
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "host.docker.internal", Port = 49154 };
                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.ExchangeDeclare(exchange: "delete_user_queue", type: ExchangeType.Fanout);

                queuename = channel.QueueDeclare().QueueName;

                //channel.QueueDeclare(queue: "delete_user_queue",
                //                     durable: false,
                //                     exclusive: false,
                //                     autoDelete: false,
                //                     arguments: null);

                channel.QueueBind(queue: queuename, exchange: "delete_user_queue", routingKey: "");

                isConnected = true;
            }
            catch (Exception e)
            {
                Trace.WriteLine("Failed to connect to rabbitmq");
            }

        }

        public override void Dispose()
        {
            if (channel.IsOpen)
            {
                channel.Close();
                connection.Close();
            }
            base.Dispose();
        }
    }
}
