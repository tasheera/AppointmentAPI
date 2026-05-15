using Microsoft.AspNetCore.Identity; // already gives id, username, email, password hash, etc. for free

namespace AppointmentAPI.Models
{
    public class AppUser : IdentityUser // with this inheritance, it creates AspNetUsers , AspNetRoles like tables
    {
        public string FullName { get; set; } = string.Empty;
    }
}