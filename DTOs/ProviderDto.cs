namespace AppointmentAPI.DTOs
{
    public class ProviderDto
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
    }
}