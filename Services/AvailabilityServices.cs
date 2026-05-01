using Hotel_Room_Reservation_System.DataAccessLayer;
using Hotel_Room_Reservation_System.Interfaces;
using Hotel_Room_Reservation_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Room_Reservation_System.Services
{
    public class AvailabilityServices: IAvailabilityService
    {
        private readonly AppDbContext _dbContext;

        public AvailabilityServices(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId,DateTime checkIn,DateTime checkOut)
        {
            var isBooked = await _dbContext.ReservationRooms.Include(r => r.Reservation)
                .AnyAsync(r => r.RoomId == roomId && r.Reservation != null &&
                 r.Reservation.Status == "Confirmed" && checkIn < r.Reservation.CheckOutDate && checkOut > r.Reservation.CheckOutDate);

            return !isBooked;
        }

        public async Task<List<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
        {
            var bookedRoomIds = await _dbContext.ReservationRooms.Include(r => r.Reservation)
                .Where(r => r.Reservation != null && r.Reservation.Status == "Confirmed" &&
                 checkIn < r.Reservation.CheckOutDate && checkOut > r.Reservation.CheckInDate)
                .Select(r => r.RoomId)
                .ToListAsync();

            var availableRooms = await _dbContext.Rooms
                .Where(r => r.IsActive && !bookedRoomIds.Contains(r.RoomId))
                .ToListAsync();

            return availableRooms;
        }
    }
}
