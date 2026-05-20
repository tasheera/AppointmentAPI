using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AppointmentAPI.Data;
using AppointmentAPI.DTOs;
using AppointmentAPI.Models;
using AppointmentAPI.Services;

namespace AppointmentAPI.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize]                    // entire controller is protected
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IEmailService _emailService;


        public BookingsController(AppDbContext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        // create new booking
        [HttpPost]
        public async Task<IActionResult> Book(BookingDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);// get user id from JWT 

            if (userId == null)
                return Unauthorized(new { message = "Invalid token" });

            // check if slot exists
            var slot = await _db.Slots
            .Include(s => s.Service)
            .Include(s => s.Provider)
            .FirstOrDefaultAsync(s => s.Id == dto.SlotId);

            if (slot == null)
                return NotFound(new { message = "Slot not found" });

            // checked if slot is already booked
            if (slot.IsBooked)
                return BadRequest(new { message = "Slot is already booked" });


            var existingBooking = await _db.Bookings
                .AnyAsync(b => b.UserId == userId && b.SlotId == dto.SlotId);// prevent user from double-booking the same slot

            if (existingBooking)
                return BadRequest(new { message = "You already booked this slot" });


            slot.IsBooked = true;

            var booking = new Booking
            {
                UserId = userId,
                SlotId = dto.SlotId,
                BookedAt = DateTime.UtcNow
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            var user = await _db.Users.FindAsync(userId);
            if (user != null)
            {
                await _emailService.SendBookingConfirmationAsync(// send confirmation email
                    user.Email!,
                    user.FullName,
                    slot.Service.Name,
                    slot.Provider.Name,
                    slot.Provider.Location,
                    slot.StartTime,
                    slot.EndTime
                );
            }

            return Ok(new
            {
                message = "Booking confirmed",
                bookingId = booking.Id,
                slotId = booking.SlotId,
                bookedAt = booking.BookedAt
            });
        }


        // get user's bookings
        [HttpGet("my")]
        public async Task<IActionResult> MyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized(new { message = "Invalid token" });

            var bookings = await _db.Bookings
                .Include(b => b.Slot)
                    .ThenInclude(s => s.Service)
                .Where(b => b.UserId == userId)
                .Select(b => new
                {
                    b.Id,
                    b.BookedAt,
                    Slot = new
                    {
                        b.Slot.Id,
                        b.Slot.StartTime,
                        b.Slot.EndTime,
                        Service = new
                        {
                            b.Slot.Service.Id,
                            b.Slot.Service.Name
                        }
                    }
                })
                .ToListAsync();

            return Ok(bookings);
        }


        // cancel booking
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var booking = await _db.Bookings
                .Include(b => b.Slot)
                    .ThenInclude(s => s.Service)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            booking.Slot.IsBooked = false;

            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();


            // send cancellation email
            var user = await _db.Users.FindAsync(userId);
            if (user != null)
            {
                await _emailService.SendBookingCancellationAsync(
                    user.Email!,
                    user.FullName,
                    booking.Slot.Service.Name,
                    booking.Slot.StartTime
                );
            }

            return Ok(new { message = "Booking cancelled successfully" });
        }
    }
}