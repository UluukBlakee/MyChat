namespace MyChat.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string? MessageText { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime DepartureDate { get; set; }
    }
}
