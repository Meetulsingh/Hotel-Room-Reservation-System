using Hotel_Room_Reservation_System.DataAccessLayer;
using Hotel_Room_Reservation_System.DTOs;
using Hotel_Room_Reservation_System.Interfaces;
using Hotel_Room_Reservation_System.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Room_Reservation_System.Services
{
    public class RoomServices : IRoomServices
    {
        private readonly AppDbContext _dbContext;

        public RoomServices(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Room> AddRoomAsync(RoomDto roomDto)
        {
            var room = new Room
            {
                RoomNumber = roomDto.RoomNumber,
                RoomType = roomDto.RoomType,
                Description = roomDto.Description,
                PricePerNight = roomDto.PricePerNight,
                Capacity = roomDto.Capacity,
                IsActive = roomDto.IsActive
            };
            _dbContext.Rooms.Add(room);
            await _dbContext.SaveChangesAsync();
            return room;
        }

        public async Task<List<Room>> GetAllRoomsAsync()
        {
            return await _dbContext.Rooms.Where(r => r.IsActive ).ToListAsync();
        }

        public async Task<Room?> GetRoomByIdAsync(int id)
        {
            var room =  await _dbContext.Rooms.FindAsync(id);

            if (room == null || !room.IsActive)
            {
                return null;
            }

            return room;
        }

        public async Task<Room?> UpdateRoomAsync(int id, RoomDto roomDto)
        {
            var room = await _dbContext.Rooms.FindAsync(id);
            if (room == null)
            {
                return null;
            }
            room.RoomNumber = roomDto.RoomNumber;
            room.RoomType = roomDto.RoomType;
            room.Description = roomDto.Description;
            room.PricePerNight = roomDto.PricePerNight;
            room.Capacity = roomDto.Capacity;
            room.IsActive = roomDto.IsActive;
            await _dbContext.SaveChangesAsync();
            return room;

        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _dbContext.Rooms.FindAsync(id);
            if (room == null)
            {
                return false;
            }
            _dbContext.Rooms.Remove(room);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
        
    }
