namespace WarehouseManagementSystem.Models
{
    public class NotificationService : INotificationService
    {
        public void SendNotification(string subject, string message)
        {
            Console.WriteLine(subject);
            Console.WriteLine(message);
        }
    }
}
