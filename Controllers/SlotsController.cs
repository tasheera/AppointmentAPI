using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentAPI.Data;
using Microsoft.AspNetCore.Authorization;
using AppointmentAPI.Models;
using AppointmentAPI.DTOs;

namespace AppointmentAPI.Controllers
{
    [ApiController]
    [Route("api/slots")]
    public class SlotsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public SlotsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(
            [FromQuery] DateOnly date,
            [FromQuery] int? providerId,
            [FromQuery] int? serviceId)
        {
            var query = _db.Slots
                .Include(s => s.Service)
                .Include(s => s.Provider)
                .Where(s => DateOnly.FromDateTime(s.StartTime) == date && !s.IsBooked)
                .AsQueryable();

            if (providerId.HasValue)
                query = query.Where(s => s.ProviderId == providerId.Value);

            if (serviceId.HasValue)
                query = query.Where(s => s.ServiceId == serviceId.Value);

            var slots = await query
                .OrderBy(s => s.StartTime)
                .Select(s => new
                {
                    s.Id,
                    s.StartTime,
                    s.EndTime,
                    Service = new { s.Service.Id, s.Service.Name },
                    Provider = new
                    {
                        s.Provider.Id,
                        s.Provider.Name,
                        s.Provider.Location
                    }
                })
                .ToListAsync();

            return Ok(slots);
        }

        //protected endpoints

        // create new slot
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(SlotDto dto)
        {
            var service = await _db.Services.FindAsync(dto.ServiceId);

            if (service == null)
                return NotFound(new { message = "Service not found" });

            var slot = new Slot
            {
                ServiceId = dto.ServiceId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            _db.Slots.Add(slot);
            await _db.SaveChangesAsync();

            return Ok(slot);
        }

        //delete slot
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var slot = await _db.Slots.FindAsync(id);

            if (slot == null)
                return NotFound(new { message = "Slot not found" });

            if (slot.IsBooked)
                return BadRequest(new { message = "Cannot delete a booked slot" });

            _db.Slots.Remove(slot);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Slot deleted" });
        }
    }
}