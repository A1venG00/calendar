namespace Notifications
{
    public class NotificationRequest
    {
        public string UserId { get; set; }
        public Guid EventId { get; set; }
        public string Message { get; set; }
    }
}
