namespace GiftOfTheGivers.Models
{
    public class RequestModel
    {
        public int Id { get; set; }
        public int DonationId { get; set; }
        public Guid RequesterId { get; set; }
        public string Message { get; set; } = null!;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}
