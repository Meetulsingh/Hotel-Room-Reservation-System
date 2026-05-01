using Hotel_Room_Reservation_System.DataAccessLayer;
using Hotel_Room_Reservation_System.Interfaces;
using Hotel_Room_Reservation_System.DTOs;
using Microsoft.EntityFrameworkCore;
using Hotel_Room_Reservation_System.Models;
using System.Text.Json.Serialization;

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

        public async Task<List<ReservationDto>> GetAllReservationsAsync()
        {
            return await _dbContext.Reservations
                .Select(r => new ReservationDto
                {
                    ReservationId = r.ReservationId,
                    UserId = r.UserId,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    TotalAmount = r.TotalAmount,
                    Rooms = r.ReservationRooms.Select(rr => new RoomDto
                    {
                        RoomId = rr.RoomId,
                        RoomNumber = rr.Room.RoomNumber,
                        RoomType = rr.Room.RoomType,
                        PricePerNight = rr.Room.PricePerNight,
                        IsActive = rr.Room.IsActive
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<object?> GetReservationByIdAsync(int id,int currentUserId)
        {
            var reservation = await _dbContext.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return null;
            }
            bool isAdmin = _dbContext.Users.Where(u => u.UserId==currentUserId).Any(a => a.Role=="Admin");
            if (currentUserId == reservation.UserId ||  isAdmin)
            {
                return await _dbContext.Reservations.Where(r => r.ReservationId == id)
                    .Select(r => new ReservationDto
                    {
                        ReservationId = r.ReservationId,
                        UserId = r.UserId,
                        CheckInDate = r.CheckInDate,
                        CheckOutDate = r.CheckOutDate,
                        TotalAmount = r.TotalAmount,
                        Rooms = r.ReservationRooms.Select(rr => new RoomDto
                        {
                            RoomId = rr.RoomId,
                            RoomNumber = rr.Room.RoomNumber,
                            RoomType = rr.Room.RoomType,
                            PricePerNight = rr.Room.PricePerNight
                        }).ToList()
                    }).FirstOrDefaultAsync();
            }

            return new { Success = false, Message = "Access denied. You can only view your own reservations." };
        }

        public async Task<string> CancelReservationAsync(int id,int currentUserId)
        {
            var reservation = await _dbContext.Reservations.FindAsync(id);
            var isAdmin = await _dbContext.Users.AnyAsync(a => a.UserId==currentUserId && a.Role=="Admin");

            if (reservation == null)
            {
                return "Reservation not found";
            }

            if(reservation.UserId == currentUserId || isAdmin)
            {
                _dbContext.Reservations.Remove(reservation);
                await _dbContext.SaveChangesAsync();
                return "Reservation cancelled successfully";
            }
            return "Reservation cancellation failed. You can only cancel your own reservations.";
        }
        }
}
