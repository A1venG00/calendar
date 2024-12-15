namespace lab2.Persistance
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string MessageType { get; set; }
        public string MessageBody { get; set; }
        public bool IsSent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; } 
    }
}
