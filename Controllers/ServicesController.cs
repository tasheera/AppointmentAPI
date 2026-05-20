using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentAPI.Data;
using Microsoft.AspNetCore.Authorization;
using AppointmentAPI.Models;
using AppointmentAPI.DTOs;

namespace AppointmentAPI.Controllers
{
    [ApiController]
    [Route("api/services")]
    public class ServicesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ServicesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await _db.Services.ToListAsync();
            return Ok(services);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var service = await _db.Services.FindAsync(id);

            if (service == null)
                return NotFound(new { message = "Service not found" });

            return Ok(service);
        }

        // protected endpoints

        // create new service
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ServiceDto dto)
        {
            var service = new Service
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _db.Services.Add(service);
            await _db.SaveChangesAsync();

            return Ok(service);
        }

        // update service
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, ServiceDto dto)
        {
            var service = await _db.Services.FindAsync(id);

            if (service == null)
                return NotFound(new { message = "Service not found" });

            service.Name = dto.Name;
            service.Description = dto.Description;

            await _db.SaveChangesAsync();
            return Ok(service);
        }

        // delete service
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _db.Services.FindAsync(id);

            if (service == null)
                return NotFound(new { message = "Service not found" });

            _db.Services.Remove(service);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Service deleted" });
        }
    }
}