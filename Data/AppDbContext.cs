using Microsoft.EntityFrameworkCore;
using AppointmentAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AppointmentAPI.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>// this tells ef ccore to create identity tables 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Service> Services => Set<Service>();
        public DbSet<Provider> Providers => Set<Provider>(); 
        public DbSet<Slot> Slots => Set<Slot>();
        public DbSet<Booking> Bookings => Set<Booking>(); 
    }
}