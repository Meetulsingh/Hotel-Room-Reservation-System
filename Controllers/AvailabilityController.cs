using Hotel_Room_Reservation_System.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Room_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly IAvailabilityService _availabilityService;

        public AvailabilityController(IAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        [HttpGet("Check-Room")]
        public async Task<IActionResult> CheckRoomAvailability(int roomId, DateTime checkIn,DateTime checkOut)
        {
            try
            {
                if(checkOut<= checkIn)
                {
                    return BadRequest("Check-out date must be after check-in date.");
                }

                var isAvailable = await _availabilityService.IsRoomAvailableAsync(roomId, checkIn, checkOut);
                return Ok(new { RoomId = roomId, isAvailable = isAvailable });
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Gel-All-Rooms")]
        public async Task<IActionResult> GetAvailableRooms(DateTime checkIn, DateTime checkOut)
        {
            try
            {
                if (checkOut <= checkIn)
                {
                    return BadRequest("Check-out date must be after check-in date.");
                }
                var availableRooms = await _availabilityService.GetAvailableRoomsAsync(checkIn, checkOut);
                return Ok(availableRooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        }
}
