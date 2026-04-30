using Hotel_Room_Reservation_System.DataAccessLayer;
using Hotel_Room_Reservation_System.DTOs;
using Hotel_Room_Reservation_System.Interfaces;
using Hotel_Room_Reservation_System.Models;
using Hotel_Room_Reservation_System.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Room_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomServices _roomservices;
        public RoomController(IRoomServices roomServices)
        {
            _roomservices = roomServices;
        }

        [HttpPost]
        public async Task<IActionResult> AddRoom(RoomDto RoomDto)
        {
            try
            {
                if(RoomDto== null)
                {
                    return BadRequest("Room data is null.");
                }
                var room = await _roomservices.AddRoomAsync(RoomDto);
                return Ok(RoomDto);

            }

            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            try
            {
                var rooms = await _roomservices.GetAllRoomsAsync();
                return Ok(rooms);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            try
            {
                var room = await _roomservices.GetRoomByIdAsync(id);
                if (room == null)
                {
                    return NotFound($"Room with ID {id} not found.");
                }
                return Ok(room);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }


           }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, RoomDto roomDto)
        {
            try
            {
                if (roomDto == null)
                {
                    return BadRequest("Room data is null.");
                }
                var updatedRoom = await _roomservices.UpdateRoomAsync(id, roomDto);
                if (updatedRoom == null)
                {
                    return NotFound($"Room with ID {id} not found.");
                }
                return Ok(updatedRoom);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            try
            {
                var deleted = await _roomservices.DeleteRoomAsync(id);
                if (!deleted)
                {
                    return NotFound($"Room with ID {id} not found.");
                }
                return Ok($"Room with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        }
}
