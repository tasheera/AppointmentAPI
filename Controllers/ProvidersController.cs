using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentAPI.Data;
using AppointmentAPI.DTOs;
using AppointmentAPI.Models;

namespace AppointmentAPI.Controllers
{
    [ApiController]
    [Route("api/providers")]
    public class ProvidersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ProvidersController(AppDbContext db)
        {
            _db = db;
        }

        //api/providers?serviceId=1
        [HttpGet]//returns all providers for a service
        public async Task<IActionResult> GetByService([FromQuery] int serviceId)
        {
            var service = await _db.Services.FindAsync(serviceId);
            if (service == null)
                return NotFound(new { message = "Service not found" });

            var providers = await _db.Providers
                .Where(p => p.ServiceId == serviceId)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Location,
                    p.Bio,
                    Service = service.Name
                })
                .ToListAsync();

            return Ok(providers);
        }

        //api/providers/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var provider = await _db.Providers
                .Include(p => p.Service)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (provider == null)
                return NotFound(new { message = "Provider not found" });

            return Ok(new
            {
                provider.Id,
                provider.Name,
                provider.Location,
                provider.Bio,
                Service = provider.Service.Name
            });
        }

        //api/providers  -admin only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProviderDto dto)
        {
            var service = await _db.Services.FindAsync(dto.ServiceId);
            if (service == null)
                return NotFound(new { message = "Service not found" });

            var provider = new Provider
            {
                ServiceId = dto.ServiceId,
                Name = dto.Name,
                Location = dto.Location,
                Bio = dto.Bio
            };

            _db.Providers.Add(provider);
            await _db.SaveChangesAsync();

            return Ok(provider);
        }

        //api/providers/{id}  -admin only
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, ProviderDto dto)
        {
            var provider = await _db.Providers.FindAsync(id);
            if (provider == null)
                return NotFound(new { message = "Provider not found" });

            provider.Name = dto.Name;
            provider.Location = dto.Location;
            provider.Bio = dto.Bio;

            await _db.SaveChangesAsync();
            return Ok(provider);
        }

        //api/providers/{id} admin only
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var provider = await _db.Providers.FindAsync(id);
            if (provider == null)
                return NotFound(new { message = "Provider not found" });

            // check if provider has any booked slots
            var hasBookedSlots = await _db.Slots
                .AnyAsync(s => s.ProviderId == id && s.IsBooked);

            if (hasBookedSlots)
                return BadRequest(new 
                { 
                    message = "Cannot delete provider with active bookings" 
                });

            _db.Providers.Remove(provider);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Provider deleted" });
        }
    }
}