# eHoteloo

eHoteloo is a hotel room booking and management system built with **ASP.NET Core MVC** (.NET 9) and **Microsoft SQL Server**.

It allows guests to browse and book rooms, staff to handle service requests, and admins to manage users, rooms, bookings, and payments.

## Features

- User registration, login, and email confirmation
- Password reset via email
- Browse and filter hotel rooms (type, price, capacity)
- Book rooms with automatic availability check
- User wallet balance for payments
- Hotel service requests (pending, in progress, completed, cancelled)
- Admin dashboard to manage users, rooms, bookings, payments, and services
- Staff panel to manage service requests
- Role-based access (Admin, Staff, User)

## Tech Stack

- ASP.NET Core MVC (.NET 9)
- C#
- Microsoft SQL Server + Entity Framework Core 9
- ASP.NET Core Identity
- Mapster
- MailKitSimplified (email)
- Stripe.net

## Getting Started

### Requirements

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- Microsoft SQL Server

### Run

1. Clone the repo:
```bash
git clone https://github.com/Than0s2007/eHoteloo-ASP.NET-Core.git
cd eHoteloo-ASP.NET-Core/eHoteloo
```

2. Update the connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=eHotelooDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

3. Apply migrations:
```bash
dotnet ef database update
```

4. Run the project:
```bash
dotnet run
```

## Project Structure

- `Controllers/` — Home and Account controllers
- `Areas/Admin` — admin panel
- `Areas/Staff` — staff panel
- `Areas/User` — user panel
- `Models/` — domain entities (User, Room, Booking, Payment, Service)
- `VM/` — view models
- `Data/` — ApplicationDbContext and configurations
- `Services/` — email sender
- `wwwroot/` — static files

## License

MIT License
