﻿namespace apiRabbitMQ.Interfaces
{
    internal interface INotificationService
    {
        void NotifyUser(int fromId, int told, string? content);
    }
}