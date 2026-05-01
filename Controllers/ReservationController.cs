using Hotel_Room_Reservation_System.DTOs;
using Hotel_Room_Reservation_System.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Hotel_Room_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] AddReservationDto dto)
        {
            try
            {
                var reservation = await _reservationService.CreateReservationAsync(dto);
                return Ok(reservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the reservation: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Get-All-Reservation")]
        public async Task<IActionResult> GetAllReservations()
        {
            try
            {
                var reservations = await _reservationService.GetAllReservationsAsync();
                return Ok(reservations);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving reservations: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin,Customer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationById(int id)
        {
            try
            {
                var currentUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var reservation = await _reservationService.GetReservationByIdAsync(id,currentUserId);
                if (reservation == null)
                {
                    return NotFound($"No reservation found with ID {id}");
                }
                return Ok(reservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the reservation: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin,Customer")]
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            try
            {
                var currentUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var result = await _reservationService.CancelReservationAsync(id,currentUserId);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while cancelling the reservation: {ex.Message}");
            }
        }
    }
}
