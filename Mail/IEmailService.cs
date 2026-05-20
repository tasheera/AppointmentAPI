namespace AppointmentAPI.Services
{
    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(string toEmail, string userName,
            string serviceName, string providerName, string providerLocation, DateTime startTime, DateTime endTime);

        Task SendBookingCancellationAsync(string toEmail, string userName,
            string serviceName, DateTime startTime);
    }
}