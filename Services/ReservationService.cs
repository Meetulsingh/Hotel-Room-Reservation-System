using Hotel_Room_Reservation_System.DataAccessLayer;
using Hotel_Room_Reservation_System.Interfaces;
using Hotel_Room_Reservation_System.DTOs;
using Microsoft.EntityFrameworkCore;
using Hotel_Room_Reservation_System.Models;

namespace Hotel_Room_Reservation_System.Services
{
    public class ReservationService : IReservationService
    {
        private readonly AppDbContext _dbContext;

        private readonly IAvailabilityService _availabilityService;

        public ReservationService(AppDbContext dbContext,IAvailabilityService availabilityService)
        {
            _dbContext = dbContext;
            _availabilityService = availabilityService;
        }

        public async Task<object> CreateReservationAsync(AddReservationDto dto)
        {
            if(dto.CheckOutDate <= dto.CheckInDate)
            {
                return new { Success = false, Message = "Invalid dates" };
            }

            if(dto.RoomIds == null || !dto.RoomIds.Any())
            {
                return new { Success = false, Message = "No rooms selected" };
            }

            var user = await _dbContext.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                return new { Success = false, Message = "User not found" };
            }

            foreach(var id in dto.RoomIds)
            {
                var isAvailable = await _availabilityService.IsRoomAvailableAsync(id, dto.CheckInDate, dto.CheckOutDate);
                if (!isAvailable)
                {
                    return new { Success = false, Message = $"Room {id} not available" };
                }
            }

            var rooms = await _dbContext.Rooms.Where(r => dto.RoomIds.Contains(r.RoomId) && r.IsActive).ToListAsync();

            var days = (dto.CheckOutDate - dto.CheckInDate).Days;

            var totalAmount = rooms.Sum(r => r.PricePerNight * days);

            var reservation = new Reservation
            {
                UserId = dto.UserId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                TotalAmount = totalAmount,
                Status = "Confirmed",
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Reservations.Add(reservation);
            await _dbContext.SaveChangesAsync();

            foreach(var room in rooms)
            {
                _dbContext.ReservationRooms.Add(new ReservationRoom
                {
                    ReservationId = reservation.ReservationId,
                    RoomId = room.RoomId
                });
            }

            await _dbContext.SaveChangesAsync();

            return new
            {
                Success = true,
                Message = "Reservation successfully created",
                ReservationId = reservation.ReservationId,
                TotalAmount = totalAmount
            };

        }

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            return await _dbContext.Reservations
                .Include(r => r.ReservationRooms)
                    .ThenInclude(rr => rr.Room)
                .ToListAsync();
        }

        public async Task<Reservation?> GetReservationByIdAsync(int id)
        {
            return await _dbContext.Reservations.Include(r => r.ReservationRooms).ThenInclude(rr => rr.Room).FirstOrDefaultAsync(r => r.ReservationId == id);
        }

        public async Task<string> CancelReservationAsync(int id,int currentUserId)
        {
            var reservation = await _dbContext.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return "Reservation not found";
            }

            if(reservation.UserId == currentUserId)
            {
                reservation.Status = "Cancelled";
                return "Reservation cancelled successfully";
            }
            return "Reservation cancellation failed. You can only cancel your own reservations.";
        }
        }
}
