namespace GiftOfTheGivers.Models
{
    public class EventModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Location { get; set; } = "TBD";
    }
}
