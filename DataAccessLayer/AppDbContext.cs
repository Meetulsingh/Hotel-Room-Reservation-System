using Hotel_Room_Reservation_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Room_Reservation_System.DataAccessLayer
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Room> Rooms => Set<Room>();

        public DbSet<Reservation> Reservations => Set<Reservation>();

        public DbSet<ReservationRoom> ReservationRooms => Set<ReservationRoom>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ReservationRoom>().HasOne(a => a.Reservation)
                .WithMany(r => r.ReservationRooms)
                .HasForeignKey(a => a.ReservationId);

            modelBuilder.Entity<ReservationRoom>()
                .HasOne(a => a.Room)
                .WithMany(r => r.ReservationRooms)  
                .HasForeignKey(a => a.RoomId);
        }
    }
}
