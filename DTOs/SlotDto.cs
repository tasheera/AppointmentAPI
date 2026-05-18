namespace AppointmentAPI.DTOs
{
    public class SlotDto
    {
        public int ServiceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}