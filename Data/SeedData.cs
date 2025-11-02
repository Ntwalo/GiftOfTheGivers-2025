using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Identity;

namespace GiftOfTheGivers.Data
{
    public static class SeedData
    {
        public static void Seed(InMemoryStore store, IPasswordHasher<User> hasher)
        {
            if (store.Users.Any()) return;

            // Seed Ntwalo@test.com / Password1 as Admin for bypass as requested
            var ntwalo = new User
            {
                Email = "Ntwalo@test.com",
                Role = "Admin",
                FullName = "Ntwalo Pressy"
            };
            ntwalo.PasswordHash = hasher.HashPassword(ntwalo, "Password1");
            store.Users.Add(ntwalo);

            // Sample donor
            var donor = new User
            {
                Email = "donor@example.com",
                Role = "Donor",
                FullName = "Sample Donor"
            };
            donor.PasswordHash = hasher.HashPassword(donor, "donorpass");
            store.Users.Add(donor);

            // Volunteer
            var vol = new User
            {
                Email = "volunteer@example.com",
                Role = "Volunteer",
                FullName = "Sample Volunteer"
            };
            vol.PasswordHash = hasher.HashPassword(vol, "volpass");
            store.Users.Add(vol);

            // Some donations
            store.Donations.Add(new Donation
            {
                Id = store.GetNextDonationId(),
                Title = "Canned Food Pack",
                Description = "20 cans assorted",
                Category = "Food",
                Quantity = 20,
                OwnerId = donor.Id
            });

            store.Donations.Add(new Donation
            {
                Id = store.GetNextDonationId(),
                Title = "Winter Blankets",
                Description = "10 warm blankets",
                Category = "Clothing",
                Quantity = 10,
                OwnerId = donor.Id
            });

            // sample event
            store.Events.Add(new EventModel
            {
                Id = store.GetNextEventId(),
                Name = "Relief Distribution - Zone A",
                Description = "Distribute donated items",
                Date = DateTime.UtcNow.AddDays(7),
                Location = "Community Hall"
            });

            // sample notification
            store.Notifications.Add(new Notification
            {
                Id = store.GetNextNotificationId(),
                UserId = ntwalo.Id,
                Message = "Welcome, Ntwalo. Admin account created.",
                IsRead = false
            });
        }
    }
}
