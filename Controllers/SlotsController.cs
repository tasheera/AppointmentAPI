using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentAPI.Data;

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
        public async Task<IActionResult> GetAvailableSlots([FromQuery] DateOnly date)
        {
            var slots = await _db.Slots
                .Include(s => s.Service)
                .Where(s => DateOnly.FromDateTime(s.StartTime) == date && !s.IsBooked)
                .ToListAsync();

            return Ok(slots);
        }
    }
}