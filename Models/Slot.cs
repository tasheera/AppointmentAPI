namespace AppointmentAPI.Models
{
    public class Slot
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
        public int ProviderId { get; set; }
        public Provider Provider { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsBooked { get; set; } = false;
    }
}