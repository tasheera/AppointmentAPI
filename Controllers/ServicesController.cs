using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentAPI.Data;

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
    }
}