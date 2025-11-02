using GiftOfTheGivers.Models;

namespace GiftOfTheGivers.Data
{
    public class InMemoryStore
    {
        public List<User> Users { get; } = new();
        public List<Donation> Donations { get; } = new();
        public List<EventModel> Events { get; } = new();
        public List<RequestModel> Requests { get; } = new();
        public List<Notification> Notifications { get; } = new();

        private int donationCounter = 1;
        private int eventCounter = 1;
        private int requestCounter = 1;
        private int notificationCounter = 1;

        public int GetNextDonationId() => donationCounter++;
        public int GetNextEventId() => eventCounter++;
        public int GetNextRequestId() => requestCounter++;
        public int GetNextNotificationId() => notificationCounter++;
    }
}
