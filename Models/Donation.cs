namespace GiftOfTheGivers.Models
{
    public class Donation
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Category { get; set; } = "General";
        public int Quantity { get; set; }
        public string Status { get; set; } = "Available"; // Available, Claimed, Delivered
        public Guid OwnerId { get; set; } // user who donated
    }
}
