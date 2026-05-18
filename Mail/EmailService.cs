using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using AppointmentAPI.Models;

namespace AppointmentAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendBookingConfirmationAsync(string toEmail, string userName,
            string serviceName, DateTime startTime, DateTime endTime)
        {
            var subject = "Booking Confirmed ✅";

            var body = $"""
                <h2>Hi {userName}, your booking is confirmed!</h2>
                <p><strong>Service:</strong> {serviceName}</p>
                <p><strong>Date:</strong> {startTime:dddd, MMMM d yyyy}</p>
                <p><strong>Time:</strong> {startTime:hh:mm tt} - {endTime:hh:mm tt}</p>
                <br/>
                <p>Thank you for booking with us.</p>
            """;

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendBookingCancellationAsync(string toEmail, string userName,
            string serviceName, DateTime startTime)
        {
            var subject = "Booking Cancelled ❌";

            var body = $"""
                <h2>Hi {userName}, your booking has been cancelled.</h2>
                <p><strong>Service:</strong> {serviceName}</p>
                <p><strong>Was scheduled for:</strong> {startTime:dddd, MMMM d yyyy} at {startTime:hh:mm tt}</p>
                <br/>
                <p>You can book a new slot anytime.</p>
            """;

            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var email = new MimeMessage();

            //from
            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));

            //to
            email.To.Add(MailboxAddress.Parse(toEmail));

            email.Subject = subject;

            //build email body
            email.Body = new TextPart("html") { Text = htmlBody };

            //send email
            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}