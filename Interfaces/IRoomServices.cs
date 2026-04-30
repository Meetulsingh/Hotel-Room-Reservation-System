using Hotel_Room_Reservation_System.DTOs;
using Hotel_Room_Reservation_System.Models;

namespace Hotel_Room_Reservation_System.Interfaces
{
    public interface IRoomServices
    {
        Task<Room> AddRoomAsync(RoomDto roomDto);
        Task<List<Room>> GetAllRoomsAsync();

        Task<Room?> GetRoomByIdAsync(int id);
        Task<Room?> UpdateRoomAsync(int id, RoomDto roomdto);
        Task<bool> DeleteRoomAsync(int id);
    }
}
