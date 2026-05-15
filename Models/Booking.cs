namespace AppointmentAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
        public int SlotId { get; set; }
        public Slot Slot { get; set; } = null!;
        public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    }
}