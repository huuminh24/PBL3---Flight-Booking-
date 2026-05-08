using AirlineBookingApi.Helpers;
using AirlineBookingApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AirlineBookingApi.Data;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }

  public DbSet<Role> Roles => Set<Role>();
  public DbSet<Account> Accounts => Set<Account>();
  public DbSet<Profile> Profiles => Set<Profile>();
  public DbSet<Airline> Airlines => Set<Airline>();
  public DbSet<Flight> Flights => Set<Flight>();
  public DbSet<FlightPrice> FlightPrices => Set<FlightPrice>();
  public DbSet<Seat> Seats => Set<Seat>();
  public DbSet<Booking> Bookings => Set<Booking>();
  public DbSet<BookingFlight> BookingFlights => Set<BookingFlight>();
  public DbSet<Passenger> Passengers => Set<Passenger>();
  public DbSet<Ticket> Tickets => Set<Ticket>();
  public DbSet<Payment> Payments => Set<Payment>();
  public DbSet<CancelRequest> CancelRequests => Set<CancelRequest>();
  public DbSet<Coupon> Coupons => Set<Coupon>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    var utcConverter = new ValueConverter<DateTime, DateTime>(
      v => v,                                              // write: keep as-is (already UTC)
      v => DateTime.SpecifyKind(v, DateTimeKind.Utc)       // read: re-apply UTC kind
    );

    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
      foreach (var property in entityType.GetProperties())
      {
        if (property.ClrType == typeof(DateTime))
        {
          property.SetValueConverter(utcConverter);
        }
      }
    }

    var seedCreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    const string staffPasswordHash = "$2a$11$V8uj4i.kgHj7de5nIOw8Uecocdz05pMroStyoy5pXKYWTINmNlsEW";
    const string customerPasswordHash = "$2a$11$jbGC1/QdSVzglrwxS/q3neeL8jmIynxW7YzQkFF7pkt4mADG67EeC";

    modelBuilder.Entity<Role>(entity =>
    {
      entity.ToTable("Roles");
      entity.HasKey(x => x.Id);
      entity.Property(x => x.Name).IsRequired().HasMaxLength(50);
      entity.HasIndex(x => x.Name).IsUnique();

      entity.HasData(
        new Role { Id = 1, Name = "Customer", CreatedAt = seedCreatedAt },
        new Role { Id = 2, Name = "Staff", CreatedAt = seedCreatedAt }
      );
    });

    modelBuilder.Entity<Account>(entity =>
    {
      entity.ToTable("Accounts");
      entity.HasKey(x => x.Id);
      entity.Property(x => x.Email).IsRequired().HasMaxLength(100);
      entity.Property(x => x.PasswordHash).IsRequired();
      entity.Property(x => x.IsActive).HasDefaultValue(true);
      entity.HasIndex(x => x.Email).IsUnique();

      entity.HasOne(x => x.Role)
      .WithMany(x => x.Accounts)
      .HasForeignKey(x => x.RoleId)
      .OnDelete(DeleteBehavior.Restrict);

      entity.HasData(
        new Account
        {
          Id = 1,
          Email = "staff@airline.com",
          PasswordHash = staffPasswordHash,
          IsActive = true,
          RoleId = 2,
          CreatedAt = seedCreatedAt
        },
        new Account
        {
          Id = 2,
          Email = "customer@airline.com",
          PasswordHash = customerPasswordHash,
          IsActive = true,
          RoleId = 1,
          CreatedAt = seedCreatedAt
        }
      );
    });

    modelBuilder.Entity<Profile>(entity =>
    {
      entity.ToTable("Profiles");
      entity.HasKey(x => x.Id);
      entity.Property(x => x.FullName).IsRequired().HasMaxLength(100);
      entity.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(20);
      entity.Property(x => x.Address).HasMaxLength(255);

      entity.HasOne(x => x.Account)
      .WithOne(x => x.Profile)
      .HasForeignKey<Profile>(x => x.AccountId)
      .OnDelete(DeleteBehavior.Cascade);

      entity.HasData(
        new Profile
        {
          Id = 1,
          FullName = "Staff Demo",
          PhoneNumber = "0900000001",
          Address = "Da Nang",
          AccountId = 1,
          CreatedAt = seedCreatedAt
        },
        new Profile
        {
          Id = 2,
          FullName = "Khach Hang Demo",
          PhoneNumber = "0900000002",
          Address = "Da Nang",
          AccountId = 2,
          CreatedAt = seedCreatedAt
        }
      );
    });

    modelBuilder.Entity<Airline>(entity =>
    {
      entity.ToTable("Airlines");
      entity.HasKey(x => x.Id);
      entity.Property(x => x.Code).IsRequired().HasMaxLength(10);
      entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
      entity.HasIndex(x => x.Code).IsUnique();

      entity.HasData(
        new Airline { Id = 1, Code = "VN", Name = "Vietnam Airlines", LogoColor = "#FFCC00", CreatedAt = seedCreatedAt },
        new Airline { Id = 2, Code = "VJ", Name = "VietJet Air", LogoColor = "#E91E63", CreatedAt = seedCreatedAt },
        new Airline { Id = 3, Code = "QH", Name = "Bamboo Airways", LogoColor = "#4CAF50", CreatedAt = seedCreatedAt },
        new Airline { Id = 4, Code = "BL", Name = "Jetstar Pacific", LogoColor = "#FF5722", CreatedAt = seedCreatedAt },
        new Airline { Id = 5, Code = "SQ", Name = "Singapore Airlines", LogoColor = "#F5A623", CreatedAt = seedCreatedAt },
        new Airline { Id = 6, Code = "TG", Name = "Thai Airways", LogoColor = "#9C27B0", CreatedAt = seedCreatedAt }
      );
    });

    modelBuilder.Entity<Flight>(entity =>
    {
      entity.ToTable("Flights");
      entity.HasKey(x => x.Id);
      entity.Property(x => x.FlightNumber).IsRequired().HasMaxLength(20);
      entity.Property(x => x.DepartureAirport).IsRequired().HasMaxLength(100);
      entity.Property(x => x.ArrivalAirport).IsRequired().HasMaxLength(100);
      entity.Property(x => x.Status).IsRequired().HasMaxLength(30);
      entity.HasIndex(x => x.FlightNumber).IsUnique();

      entity.HasOne(x => x.Airline)
      .WithMany(x => x.Flights)
      .HasForeignKey(x => x.AirlineId)
      .OnDelete(DeleteBehavior.SetNull);

      entity.HasData(
        new Flight
        {
          Id = 1,
          FlightNumber = "VN1001",
          DepartureAirport = "Da Nang",
          ArrivalAirport = "Ho Chi Minh",
          DepartureTime = new DateTime(2026, 5, 1, 8, 0, 0, DateTimeKind.Utc),
          ArrivalTime = new DateTime(2026, 5, 1, 9, 30, 0, DateTimeKind.Utc),
          Status = "Scheduled",
          AirlineId = 1,
          CreatedAt = seedCreatedAt
        },
        new Flight
        {
          Id = 2,
          FlightNumber = "VN1002",
          DepartureAirport = "Da Nang",
          ArrivalAirport = "Ha Noi",
          DepartureTime = new DateTime(2026, 5, 1, 13, 0, 0, DateTimeKind.Utc),
          ArrivalTime = new DateTime(2026, 5, 1, 14, 20, 0, DateTimeKind.Utc),
          Status = "Boarding",
          AirlineId = 1,
          CreatedAt = seedCreatedAt
        },
        new Flight
        {
          Id = 3,
          FlightNumber = "VN2001",
          DepartureAirport = "Ho Chi Minh",
          ArrivalAirport = "Da Nang",
          DepartureTime = new DateTime(2026, 5, 2, 10, 0, 0, DateTimeKind.Utc),
          ArrivalTime = new DateTime(2026, 5, 2, 11, 25, 0, DateTimeKind.Utc),
          Status = "Delayed",
          AirlineId = 1,
          CreatedAt = seedCreatedAt
        },
        new Flight
        {
          Id = 4,
          FlightNumber = "VN3001",
          DepartureAirport = "Ha Noi",
          ArrivalAirport = "Da Nang",
          DepartureTime = new DateTime(2026, 5, 2, 16, 30, 0, DateTimeKind.Utc),
          ArrivalTime = new DateTime(2026, 5, 2, 17, 50, 0, DateTimeKind.Utc),
          Status = "Scheduled",
          AirlineId = 1,
          CreatedAt = seedCreatedAt
        }
      // VN2002 / VN3002 are seeded via raw SQL in AddMultiCityFlightsAndPaymentCoupon migration
      // with auto-generated Ids, so they are intentionally not declared as HasData here.
      );
    });

    modelBuilder.Entity<FlightPrice>(entity =>
    {
      entity.ToTable("FlightPrices");
      entity.HasKey(x => x.Id);
      entity.Property(x => x.SeatClass).IsRequired().HasMaxLength(20);
      entity.Property(x => x.Price).HasColumnType("decimal(18,2)");

      entity.HasOne(x => x.Flight)
      .WithMany(x => x.FlightPrices)
      .HasForeignKey(x => x.FlightId)
      .OnDelete(DeleteBehavior.Cascade);

      entity.HasData(
        new FlightPrice { Id = 1, FlightId = 1, SeatClass = "Economy", Price = 1200000, CreatedAt = seedCreatedAt },
        new FlightPrice { Id = 2, FlightId = 1, SeatClass = "Business", Price = 2200000, CreatedAt = seedCreatedAt },
        new FlightPrice { Id = 3, FlightId = 2, SeatClass = "Economy", Price = 1400000, CreatedAt = seedCreatedAt },
        new FlightPrice { Id = 4, FlightId = 2, SeatClass = "Business", Price = 2500000, CreatedAt = seedCreatedAt },
        new FlightPrice { Id = 5, FlightId = 3, SeatClass = "Economy", Price = 1300000, CreatedAt = seedCreatedAt },
        new FlightPrice { Id = 6, FlightId = 3, SeatClass = "Business", Price = 2300000, CreatedAt = seedCreatedAt },
        new FlightPrice { Id = 7, FlightId = 4, SeatClass = "Economy", Price = 1500000, CreatedAt = seedCreatedAt },
        new FlightPrice { Id = 8, FlightId = 4, SeatClass = "Business", Price = 2600000, CreatedAt = seedCreatedAt }
      // FlightPrices for VN2002 / VN3002 are seeded via raw SQL in AddMultiCityFlightsAndPaymentCoupon
      // migration (looked up by FlightNumber), so they are intentionally not declared as HasData here.
      );
    });

    modelBuilder.Entity<Seat>(entity =>
    {
      entity.ToTable("Seats");
      entity.HasKey(x => x.Id);
      entity.Property(x => x.SeatNumber).IsRequired().HasMaxLength(10);
      entity.Property(x => x.SeatClass).IsRequired().HasMaxLength(20);
      entity.HasIndex(x => new { x.FlightId, x.SeatNumber }).IsUnique();

      entity.HasOne(x => x.Flight)
      .WithMany(x => x.Seats)
      .HasForeignKey(x => x.FlightId)
      .OnDelete(DeleteBehavior.Cascade);

      // Seats are seeded via raw SQL in migrations (CleanSeatsAndExpandTo84, etc.).
      // HasData removed to avoid Id conflicts with migration-generated seat rows.
      // All seats now use modern row-column format: E{row}{col} and B{row}{col}.
    });

    modelBuilder.Entity<Booking>(entity =>
    {
      entity.ToTable("Bookings");
      entity.HasKey(x => x.Id);
      entity.Property(x => x.PnrCode).IsRequired().HasMaxLength(20);
      entity.Property(x => x.BookingStatus).IsRequired().HasMaxLength(30);
      entity.Property(x => x.BookingChannel).IsRequired().HasMaxLength(30);
      entity.Property(x => x.ContactFullName).IsRequired().HasMaxLength(100);
      entity.Property(x => x.ContactEmail).IsRequired().HasMaxLength(100);
      entity.Property(x => x.ContactPhoneNumber).IsRequired().HasMaxLength(20);
      entity.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
      entity.Property(x => x.ExpiresAt).IsRequired();
      entity.HasIndex(x => x.PnrCode).IsUnique();
      entity.HasIndex(x => x.BookingStatus);

      entity.HasOne(x => x.CustomerAccount)
      .WithMany()
      .HasForeignKey(x => x.CustomerAccountId)
      .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(x => x.CreatedByAccount)
      .WithMany()
      .HasForeignKey(x => x.CreatedByAccountId)
      .OnDelete(DeleteBehavior.Restrict);
    });

    modelBuilder.Entity<BookingFlight>(entity =>
    {
      entity.ToTable("BookingFlights");
      entity.HasKey(x => x.Id);
      entity.Property(x => x.SeatClass).IsRequired().HasMaxLength(20);
      entity.Property(x => x.Price).HasColumnType("decimal(18,2)");
      entity.HasIndex(x => new { x.BookingId, x.FlightId }).IsUnique();

      entity.HasOne(x => x.Booking)
      .WithMany(x => x.BookingFlights)
      .HasForeignKey(x => x.BookingId)
      .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(x => x.Flight)
      .WithMany()
      .HasForeignKey(x => x.FlightId)
      .OnDelete(DeleteBehavior.Restrict);
    });

    modelBuilder.Entity<Passenger>(entity =>
    {
      entity.ToTable("Passengers");
      entity.HasKey(x => x.Id);
      entity.Property(x => x.FullName).IsRequired().HasMaxLength(100);
      entity.Property(x => x.Gender).IsRequired().HasMaxLength(20);
      entity.Property(x => x.PassengerType).IsRequired().HasMaxLength(20);

      entity.HasOne(x => x.Booking)
      .WithMany(x => x.Passengers)
      .HasForeignKey(x => x.BookingId)
      .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<Ticket>(entity =>
    {
      entity.ToTable("Tickets");
      entity.HasKey(x => x.Id);
      entity.Property(x => x.SeatClass).IsRequired().HasMaxLength(20);
      entity.Property(x => x.Price).HasColumnType("decimal(18,2)");
      entity.Property(x => x.TicketStatus).IsRequired().HasMaxLength(30);
      entity.HasIndex(x => x.PassengerId);
      entity.HasIndex(x => x.SeatId).IsUnique();

      entity.HasOne(x => x.Booking)
      .WithMany(x => x.Tickets)
      .HasForeignKey(x => x.BookingId)
      .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(x => x.Passenger)
      .WithMany(x => x.Tickets)
      .HasForeignKey(x => x.PassengerId)
      .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(x => x.BookingFlight)
      .WithMany()
      .HasForeignKey(x => x.BookingFlightId)
      .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(x => x.Flight)
      .WithMany()
      .HasForeignKey(x => x.FlightId)
      .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(x => x.Seat)
      .WithMany()
      .HasForeignKey(x => x.SeatId)
      .OnDelete(DeleteBehavior.Restrict);
    });

    modelBuilder.Entity<Payment>(entity =>
    {
      entity.ToTable("Payments");
      entity.HasKey(x => x.Id);
      entity.Property(x => x.PaymentMethod).IsRequired().HasMaxLength(30);
      entity.Property(x => x.Amount).HasColumnType("decimal(18,2)");
      entity.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
      entity.Property(x => x.CouponCode).HasMaxLength(50);
      entity.Property(x => x.PaymentStatus).IsRequired().HasMaxLength(30);
      entity.Property(x => x.PaymentReference).HasMaxLength(50);

      entity.HasOne(x => x.Booking)
      .WithMany()
      .HasForeignKey(x => x.BookingId)
      .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<CancelRequest>(entity =>
    {
      entity.ToTable("CancelRequests");
      entity.HasKey(x => x.Id);
      entity.Property(x => x.Reason).IsRequired().HasMaxLength(500);
      entity.Property(x => x.CancelStatus).IsRequired().HasMaxLength(30);
      entity.Property(x => x.RefundAmount).HasColumnType("decimal(18,2)");
      entity.Property(x => x.RowVersion).IsRowVersion();

      entity.HasOne(x => x.Ticket)
      .WithMany(x => x.CancelRequests)
      .HasForeignKey(x => x.TicketId)
      .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(x => x.RequestedByAccount)
      .WithMany()
      .HasForeignKey(x => x.RequestedByAccountId)
      .OnDelete(DeleteBehavior.Restrict);
    });

    modelBuilder.Entity<Coupon>(entity =>
    {
      entity.ToTable("Coupons");
      entity.HasKey(x => x.Id);
      entity.Property(x => x.Code).IsRequired().HasMaxLength(50);
      entity.Property(x => x.DiscountPercent).HasColumnType("decimal(5,2)");
      entity.HasIndex(x => x.Code).IsUnique();

      entity.HasData(
        new Coupon { Id = 1, Code = "WELCOME10", DiscountPercent = 10, MaxUses = 100, CurrentUses = 0, ExpiryDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc), IsActive = true, CreatedAt = seedCreatedAt },
        new Coupon { Id = 2, Code = "SUMMER20", DiscountPercent = 20, MaxUses = 50, CurrentUses = 0, ExpiryDate = new DateTime(2026, 8, 31, 0, 0, 0, DateTimeKind.Utc), IsActive = true, CreatedAt = seedCreatedAt },
        new Coupon { Id = 3, Code = "VIP30", DiscountPercent = 30, MaxUses = 10, CurrentUses = 0, ExpiryDate = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc), IsActive = true, CreatedAt = seedCreatedAt }
      );
    });
  }
}