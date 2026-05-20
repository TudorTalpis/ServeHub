using TWeb.DataAccess.Context;
using TWeb.Domain.Entities.Booking;
using TWeb.Domain.Entities.Provider;
using TWeb.Domain.Entities.Service;
using TWeb.Domain.Entities.User;
using TWeb.Domain.Enums;

namespace TWeb.API;

public static class DbSeeder
{
    public static void Seed()
    {
        SeedUsersAndNotifications();
        SeedCategoriesServicesAndReviews();
        SeedProvidersAndAvailability();
        SeedBookings();
    }

    private static string Hash(string plain) => BCrypt.Net.BCrypt.HashPassword(plain);

    private static string FutureDate(int daysFromNow) =>
        DateTime.UtcNow.AddDays(daysFromNow).ToString("yyyy-MM-dd");

    private static string NowIso() => DateTime.UtcNow.ToString("o");

    private static void SeedUsersAndNotifications()
    {
        using var db = new UserContext();
        if (db.AppUsers.Any()) return;

        db.AppUsers.AddRange(
            new AppUser { Id = "user1",  Name = "Alex Johnson",  Email = "alex@demo.com",  Phone = "0712 345 678", Password = Hash("Prezentare1"), Role = Role.USER,     Avatar = "" },
            new AppUser { Id = "prov1",  Name = "Maria Garcia",  Email = "maria@demo.com", Phone = "0711 111 111", Password = Hash("Prezentare1"), Role = Role.PROVIDER, Avatar = "" },
            new AppUser { Id = "prov2",  Name = "James Wilson",  Email = "james@demo.com", Phone = "0722 222 222", Password = Hash("Prezentare1"), Role = Role.PROVIDER, Avatar = "" },
            new AppUser { Id = "prov3",  Name = "Sarah Lee",     Email = "sarah@demo.com", Phone = "0733 333 333", Password = Hash("Prezentare1"), Role = Role.PROVIDER, Avatar = "" },
            new AppUser { Id = "admin1", Name = "Admin User",    Email = "admin@demo.com", Phone = "0744 444 444", Password = Hash("Prezentare1"), Role = Role.ADMIN,    Avatar = "" }
        );

        db.AppNotifications.AddRange(
            new AppNotification
            {
                Id = "n1",
                UserId = "user1",
                Type = NotificationType.booking_success,
                Title = "Booking Confirmed",
                Message = $"Your cleaning session with Maria's Cleaning Co. is confirmed for {FutureDate(3)}",
                Read = false,
                CreatedAt = NowIso()
            },
            new AppNotification
            {
                Id = "n2",
                UserId = "prov1",
                Type = NotificationType.new_booking,
                Title = "New Booking",
                Message = $"Alex Johnson booked Standard Cleaning on {FutureDate(3)}",
                Read = false,
                CreatedAt = NowIso()
            },
            new AppNotification
            {
                Id = "n3",
                UserId = "admin1",
                Type = NotificationType.application_submitted,
                Title = "New Provider Application",
                Message = "A new provider application has been submitted.",
                Read = true,
                CreatedAt = NowIso()
            }
        );

        db.SaveChanges();
    }

    private static void SeedCategoriesServicesAndReviews()
    {
        using var db = new ServiceContext();
        if (db.Categories.Any()) return;

        db.Categories.AddRange(
            new Category { Id = "cat1", Name = "Home Cleaning",     Icon = "Sparkles", Description = "Professional cleaning services",   Color = "primary" },
            new Category { Id = "cat2", Name = "Personal Training", Icon = "Dumbbell", Description = "Fitness & wellness coaching",      Color = "accent" },
            new Category { Id = "cat3", Name = "Photography",       Icon = "Camera",   Description = "Professional photo sessions",     Color = "info" },
            new Category { Id = "cat4", Name = "Tutoring",          Icon = "BookOpen", Description = "Academic tutoring & mentoring",   Color = "success" },
            new Category { Id = "cat5", Name = "Pet Care",          Icon = "Heart",    Description = "Pet sitting & grooming",          Color = "warning" },
            new Category { Id = "cat6", Name = "Handyman",          Icon = "Wrench",   Description = "Home repairs & maintenance",      Color = "destructive" }
        );

        db.Services.AddRange(
            new Service { Id = "svc1", ProviderId = "pp1", Title = "Standard Cleaning",         Description = "Full home cleaning — kitchen, bathrooms, living areas",     Price =  80, Duration = 120, BufferMinutes = 15, CategoryId = "cat1" },
            new Service { Id = "svc2", ProviderId = "pp1", Title = "Deep Cleaning",             Description = "Intensive cleaning including appliances and hard-to-reach", Price = 150, Duration = 240, BufferMinutes = 30, CategoryId = "cat1" },
            new Service { Id = "svc3", ProviderId = "pp2", Title = "Personal Training Session", Description = "1-on-1 training with custom exercises",                     Price =  60, Duration =  60, BufferMinutes = 15, CategoryId = "cat2" },
            new Service { Id = "svc4", ProviderId = "pp2", Title = "Nutrition Consultation",    Description = "Custom meal plan and dietary guidance",                     Price =  45, Duration =  45, BufferMinutes = 15, CategoryId = "cat2" },
            new Service { Id = "svc5", ProviderId = "pp3", Title = "Portrait Session",          Description = "Professional portrait photography — 20 edited photos",      Price = 120, Duration =  90, BufferMinutes = 20, CategoryId = "cat3" },
            new Service { Id = "svc6", ProviderId = "pp3", Title = "Event Photography",         Description = "Full event coverage — unlimited photos",                    Price = 350, Duration = 240, BufferMinutes = 30, CategoryId = "cat3" }
        );

        db.Reviews.Add(new Review
        {
            Id = "rev1",
            BookingId = "bk2",
            ProviderId = "pp1",
            UserId = "user1",
            Rating = 5,
            Comment = "Excellent service! Very thorough and professional. Highly recommend!",
            CreatedAt = DateTime.UtcNow.AddDays(-2).ToString("o"),
            UserName = "Alex Johnson"
        });

        db.SaveChanges();
    }

    private static void SeedProvidersAndAvailability()
    {
        using var db = new ProviderContext();
        if (db.ProviderProfiles.Any()) return;

        db.ProviderProfiles.AddRange(
            new ProviderProfile
            {
                Id = "pp1",
                UserId = "prov1",
                Name = "Maria's Cleaning Co.",
                Slug = "MariaCleaning",
                Description = "Top-rated cleaning service with 5+ years of experience. Eco-friendly products and meticulous attention to detail.",
                CategoryIds = new List<string> { "cat1" },
                PendingCategoryNames = new List<string>(),
                Avatar = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=200&h=200&fit=crop",
                CoverPhoto = "https://images.unsplash.com/photo-1581578731548-c64695cc6952?w=800&h=400&fit=crop",
                GalleryPhotos = new List<string>
                {
                    "https://images.unsplash.com/photo-1558618666-fcd25c85f82e?w=400&h=300&fit=crop",
                    "https://images.unsplash.com/photo-1584820927498-cfe5211fd8bf?w=400&h=300&fit=crop",
                    "https://images.unsplash.com/photo-1527515637462-cee1652e65b1?w=400&h=300&fit=crop"
                },
                Phone = "555-0101",
                Location = "Downtown",
                DefaultServiceBufferMinutes = 15,
                AutoConfirm = false,
                Rating = 4.9,
                ReviewCount = 127,
                Featured = true,
                Sponsored = false,
                Blocked = false
            },
            new ProviderProfile
            {
                Id = "pp2",
                UserId = "prov2",
                Name = "FitLife with James",
                Slug = "FitLifeJames",
                Description = "Certified personal trainer specializing in strength training and weight loss. Customized programs for all levels.",
                CategoryIds = new List<string> { "cat2" },
                PendingCategoryNames = new List<string>(),
                Avatar = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=200&h=200&fit=crop",
                CoverPhoto = "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?w=800&h=400&fit=crop",
                GalleryPhotos = new List<string>
                {
                    "https://images.unsplash.com/photo-1571019614242-c5c5dee9f50b?w=400&h=300&fit=crop",
                    "https://images.unsplash.com/photo-1517836357463-d25dfeac3438?w=400&h=300&fit=crop"
                },
                Phone = "555-0102",
                Location = "Midtown",
                DefaultServiceBufferMinutes = 15,
                AutoConfirm = false,
                Rating = 4.7,
                ReviewCount = 89,
                Featured = false,
                Sponsored = true,
                Blocked = false
            },
            new ProviderProfile
            {
                Id = "pp3",
                UserId = "prov3",
                Name = "Sarah's Photography",
                Slug = "SarahPhoto",
                Description = "Award-winning photographer for weddings, portraits, and events. Natural light specialist.",
                CategoryIds = new List<string> { "cat3" },
                PendingCategoryNames = new List<string>(),
                Avatar = "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=200&h=200&fit=crop",
                CoverPhoto = "https://images.unsplash.com/photo-1452587925148-ce544e77e70d?w=800&h=400&fit=crop",
                GalleryPhotos = new List<string>
                {
                    "https://images.unsplash.com/photo-1519741497674-611481863552?w=400&h=300&fit=crop",
                    "https://images.unsplash.com/photo-1511285560929-80b456fea0bc?w=400&h=300&fit=crop",
                    "https://images.unsplash.com/photo-1606216794074-735e91aa2c92?w=400&h=300&fit=crop",
                    "https://images.unsplash.com/photo-1554080353-a576cf803bda?w=400&h=300&fit=crop"
                },
                Phone = "555-0103",
                Location = "Uptown",
                DefaultServiceBufferMinutes = 20,
                AutoConfirm = false,
                Rating = 4.8,
                ReviewCount = 64,
                Featured = false,
                Sponsored = false,
                Blocked = false
            }
        );

        foreach (var d in new[] { 1, 2, 3, 4, 5 })
            db.Availabilities.Add(new Availability { Id = $"av-pp1-{d}", ProviderId = "pp1", Weekday = d, StartTime = "08:00", EndTime = "17:00", SlotMinutes = 120, BufferMinutes = 30 });

        foreach (var d in new[] { 1, 2, 3, 4, 5, 6 })
            db.Availabilities.Add(new Availability { Id = $"av-pp2-{d}", ProviderId = "pp2", Weekday = d, StartTime = "06:00", EndTime = "20:00", SlotMinutes = 60, BufferMinutes = 15 });

        foreach (var d in new[] { 0, 2, 3, 4, 5, 6 })
            db.Availabilities.Add(new Availability { Id = $"av-pp3-{d}", ProviderId = "pp3", Weekday = d, StartTime = "09:00", EndTime = "18:00", SlotMinutes = 90, BufferMinutes = 30 });

        db.SaveChanges();
    }

    private static void SeedBookings()
    {
        using var db = new BookingContext();
        if (db.Bookings.Any()) return;

        db.Bookings.AddRange(
            new Booking
            {
                Id = "bk1",
                UserId = "user1",
                ProviderId = "pp1",
                ServiceId = "svc1",
                Date = FutureDate(3),
                StartTime = "08:00",
                EndTime = "10:00",
                Status = BookingStatus.CONFIRMED,
                CreatedAt = NowIso(),
                UserName = "Alex Johnson",
                UserPhone = "0712 345 678"
            },
            new Booking
            {
                Id = "bk2",
                UserId = "user1",
                ProviderId = "pp2",
                ServiceId = "svc3",
                Date = FutureDate(-2),
                StartTime = "10:00",
                EndTime = "11:00",
                Status = BookingStatus.COMPLETED,
                CreatedAt = NowIso(),
                UserName = "Alex Johnson",
                UserPhone = "0712 345 678"
            }
        );

        db.SaveChanges();
    }
}
