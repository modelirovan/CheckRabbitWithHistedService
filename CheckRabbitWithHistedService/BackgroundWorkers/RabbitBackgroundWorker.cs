using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CheckRabbitWithHistedService.BackgroundWorkers
{
    public class RabbitBackgroundWorker : IHostedService, IDisposable
    {
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string QueueName = "myqueue";
        private const string ExchangeName = "";
        private const bool IsDurable = true;
        //The two below settings are just to illustrate how they can be used but we are not using them in
        //this sample as we will use the defaults
        private const string VirtualHost = "";
        private int Port = 0;

        public delegate void OnReceiveMessage(string message);

        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _model;
        private EventingBasicConsumer _consumer;

        public RabbitBackgroundWorker()
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = HostName,
                UserName = UserName,
                Password = Password
            };

            if (string.IsNullOrEmpty(VirtualHost) == false)
            {
                _connectionFactory.VirtualHost = VirtualHost;
            }

            if (Port > 0)
            {
                _connectionFactory.Port = Port;
            }

            _connection = _connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
            _model.BasicQos(0, 1, false);
            _consumer = new EventingBasicConsumer(_model);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            _model.QueueDeclare(queue: "hello",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);


            _consumer.Received += (model, ea) =>
                
            {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine("Message Recieved - {0}", message);
                    _model.BasicAck(ea.DeliveryTag, false);
                };

                _model.BasicConsume(QueueName, false, _consumer);

                await Task.Delay(1000);

          



        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
