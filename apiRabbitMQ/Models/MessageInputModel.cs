using System;

namespace apiRabbitMQ.Models
{
    public class MessageInputModel
    {
        public int FromId { get; set; }
        public int Told { get; set; }
        public String? Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}