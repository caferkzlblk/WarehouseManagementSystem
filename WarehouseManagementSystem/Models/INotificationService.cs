namespace WarehouseManagementSystem.Models
{
    public interface INotificationService
    {
        void SendNotification(string subject, string message);
    }
}
