using Microsoft.EntityFrameworkCore;
using AppointmentAPI.Models;

namespace AppointmentAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Service> Services => Set<Service>();
    }
}