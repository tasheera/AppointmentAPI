# BookIt — Appointment Booking API

A REST API built with ASP.NET Core 8 for managing service-based appointment bookings. Built to learn ASP.NET Core — covers auth, roles, EF Core relationships, and email notifications.

---

## Tech Stack

- ASP.NET Core 8
- PostgreSQL + Entity Framework Core
- ASP.NET Core Identity
- JWT Authentication
- MailKit (email notifications)

---

## Features

**Public**
- View all services
- View providers by service
- View available slots by date, provider, or service

**Customer (requires login)**
- Book an available slot
- View own bookings
- Cancel own booking
- Email confirmation on booking and cancellation

**Admin**
- Create, update, delete services and providers
- Create slots manually
- View all slots by date
- Delete available (unbooked) slots

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL

### Setup

1. Clone the repo
```bash
git clone https://github.com/your-username/bookit-api
cd bookit-api
```

2. Update `appsettings.json`
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=bookitdb;Username=your_user;Password=your_password"
},
"Jwt": {
  "Key": "your-secret-key-min-32-chars-long",
  "Issuer": "AppointmentAPI",
  "Audience": "AppointmentAPI"
},
"EmailSettings": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "SenderEmail": "your-gmail@gmail.com",
  "SenderName": "Appointment App",
  "Password": "your-gmail-app-password"
}
```

> For Gmail, generate an App Password under Google Account → Security → App Passwords.

3. Run migrations
```bash
dotnet ef database update
```

4. Run the app
```bash
dotnet run
```

5. Open Swagger at `http://localhost:5000/swagger` (or the assigned port)

---

## Deployment

- API hosted on **DigitalOcean** (Docker container)
- Database on **Neon DB** (managed PostgreSQL)

