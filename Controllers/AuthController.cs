using BCrypt.Net;
using Hotel_Room_Reservation_System.DataAccessLayer;
using Hotel_Room_Reservation_System.DTOs;
using Hotel_Room_Reservation_System.Models;
using Hotel_Room_Reservation_System.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Room_Reservation_System.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext dbContext, JwtService jwtService)
    {
        _dbContext = dbContext;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var userExists = await _dbContext.Users
                .AnyAsync(u => u.Email == dto.Email);

            if (userExists)
                return BadRequest("Email already exists");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = string.IsNullOrWhiteSpace(dto.Role) ? "Customer" : dto.Role
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                Message = "User registered successfully",
                UserId = user.UserId,
                Role = user.Role
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error while registering user: {ex.Message}");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return Unauthorized("Invalid email or password");

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
                return Unauthorized("Invalid email or password");

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                Message = "Login successful",
                Token = token,
                UserId = user.UserId,
                Role = user.Role
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error while login: {ex.Message}");
        }
    }
}
