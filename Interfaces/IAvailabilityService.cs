using Hotel_Room_Reservation_System.Models;

namespace Hotel_Room_Reservation_System.Interfaces
{
    public interface IAvailabilityService
    {
        Task<bool> IsRoomAvailableAsync(int roomId,DateTime checkInDate, DateTime checkOutDate);
        Task<List<Room>> GetAvailableRoomsAsync(DateTime checkInDate, DateTime checkOutDate);
    }
}
