using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AppointmentAPI.Data;
using AppointmentAPI.DTOs;
using AppointmentAPI.Models;

namespace AppointmentAPI.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize]                    // entire controller is protected
    public class BookingsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public BookingsController(AppDbContext db)
        {
            _db = db;
        }

        // POST /api/bookings
        [HttpPost]
        public async Task<IActionResult> Book(BookingDto dto)
        {
            // Get userId from JWT token — never trust client-sent userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized(new { message = "Invalid token" });

            // Check if slot exists
            var slot = await _db.Slots.FindAsync(dto.SlotId);

            if (slot == null)
                return NotFound(new { message = "Slot not found" });

            // Check if slot is already booked
            if (slot.IsBooked)
                return BadRequest(new { message = "Slot is already booked" });

            // Check if user already has a booking for this slot
            var existingBooking = await _db.Bookings
                .AnyAsync(b => b.UserId == userId && b.SlotId == dto.SlotId);

            if (existingBooking)
                return BadRequest(new { message = "You already booked this slot" });

            // Mark slot as booked
            slot.IsBooked = true;

            // Create booking
            var booking = new Booking
            {
                UserId = userId,
                SlotId = dto.SlotId,
                BookedAt = DateTime.UtcNow
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Booking confirmed",
                bookingId = booking.Id,
                slotId = booking.SlotId,
                bookedAt = booking.BookedAt
            });
        }

        // GET /api/bookings/my
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

        // DELETE /api/bookings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var booking = await _db.Bookings
                .Include(b => b.Slot)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            // Free up the slot
            booking.Slot.IsBooked = false;

            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Booking cancelled successfully" });
        }
    }
}