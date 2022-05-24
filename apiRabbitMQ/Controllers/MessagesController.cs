using System.Text;
using apiRabbitMQ.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace apiRabbitMQ.Controllers  {
    [Route("api/[Controller]")]
    public class MessagesController : ControllerBase {
        private readonly ConnectionFactory _factory;
        private const string QUEUE_NAME = "messages";

        public MessagesController()
        {
            _factory = new ConnectionFactory{
                HostName = "localhost"
            };
        }
        
        [HttpPost]
        public IActionResult PostMessage([FromBody] MessageInputModel message){
            using (var connection = _factory.CreateConnection()){
                using (var channel = connection.CreateModel()){
                    channel.QueueDeclare(
                        queue: QUEUE_NAME,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    var stringfiedMessage = JsonConvert.SerializeObject(message);
                    var bytesMessage = Encoding.UTF8.GetBytes(stringfiedMessage);

                    channel.BasicPublish(
                            exchange: "",
                            routingKey: QUEUE_NAME,
                            basicProperties:null,
                            body: bytesMessage
                        );
                }
            }
            return Accepted();
        }
    }
}