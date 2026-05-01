using Hotel_Room_Reservation_System.DTOs;
using Hotel_Room_Reservation_System.Models;

namespace Hotel_Room_Reservation_System.Interfaces
{
    public interface IReservationService
    {
        Task<object> CreateReservationAsync(AddReservationDto dto);
        Task<List<Reservation>> GetAllReservationsAsync();
        Task<Reservation?> GetReservationByIdAsync(int id);
        Task<string> CancelReservationAsync(int id,int currentUserId);
    }
}
