namespace apiRabbitMQ.Interfaces
{
    public interface INotificationService
    {
        void NotifyUser(int fromId, int told, string? content);
    }
}