# 🏨 eHoteloo — Hotel Booking Management System

> A complete, role-based hotel room booking and management platform built with **ASP.NET Core MVC** (.NET 9) and **Microsoft SQL Server**.  
> Guests browse and book rooms, staff handle service requests, and admins oversee users, rooms, bookings, and payments — all in one elegant application.

[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?logo=asp.net)](https://dotnet.microsoft.com/apps/aspnet)
[![Microsoft SQL Server](https://img.shields.io/badge/SQL%20Server-EF%20Core-CC2927?logo=microsoft-sql-server)](https://learn.microsoft.com/en-us/ef/core/)
[![Stripe](https://img.shields.io/badge/Payments-Stripe-008CDD?logo=stripe)](https://stripe.com/)
[![Mapster](https://img.shields.io/badge/Mapping-Mapster-563D7C)](https://github.com/MapsterMapper/Mapster)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## 📸 Preview

> _Coming soon — add screenshots here to showcase the home page, room listing, booking flow, admin dashboard, and staff panel._

```markdown
# Example screenshot block
![Home Page](https://your-image-url/home.png)
![Room Listing](https://your-image-url/rooms.png)
![Admin Dashboard](https://your-image-url/admin.png)
```

---

## 🚀 Features

### 👥 Authentication & Accounts
- User registration with **email confirmation** (secure token-based flow)
- Login / Logout with cookie authentication
- **Forgot password** & **Reset password** via email
- Account lockout after failed attempts (brute-force protection)
- Email uniqueness enforced
- Custom user profile: first name, last name, phone, balance, identity photo, verification status

### 🏠 Room Booking (Guests)
- Browse available rooms (Single / Double / Suite)
- Room details with images and pricing
- Filter rooms by **type, price range, and capacity**
- Sort results (price ascending/descending, by type)
- Real-time availability check (prevents overlapping bookings)
- Instant booking with **booking number** generation (e.g. `BK-A1B2C3D4`)
- Booking confirmation page with payment summary
- Deducts booking cost from user wallet balance atomically (database transaction)

### 💳 Payments & Wallet
- Each user has a **balance (wallet)** used to pay for bookings
- Payment methods: **Cash** & **Online Payment**
- Payment statuses: Pending / Success / Failed
- Linked to bookings for full traceability

### 🛎️ Hotel Services (Staff ↔ User)
- Users can request services (e.g. room service, housekeeping, maintenance)
- Service lifecycle: **Pending → In Progress → Completed → Cancelled**
- Staff can view, update, and manage service requests
- Services can be linked to payments

### ⚙️ Administration (Admin Area)
- **Dashboard** — overview of the hotel operation
- **Rooms Management** — create, edit, activate/deactivate rooms, upload images
- **Users Management** — view all users, activate/deactivate accounts, verify identity
- **Bookings Management** — monitor all bookings and their statuses (Pending, Confirmed, Checked In, Checked Out, Cancelled)
- **Payments Management** — oversee transactions
- **Services Management** — supervise service requests across the hotel

### 🏗️ Architecture
- **ASP.NET Core MVC** pattern with **Areas** (Admin / Staff / User)
- **Entity Framework Core 9** with SQL Server (Code First + Migrations)
- **ASP.NET Core Identity** (custom `User : IdentityUser<int>`) with roles: `Admin`, `Staff`, `User`
- **Mapster** for object-to-object mapping (ViewModels ↔ Models)
- Repository-style access via `ApplicationDbContext`
- Role-based authorization (`[Authorize(Roles = "Admin")]`, etc.)

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|------------|
| Framework | ASP.NET Core MVC (.NET 9.0) |
| Language | C# 13 |
| Database | Microsoft SQL Server |
| ORM | Entity Framework Core 9 (Code First) |
| Identity | ASP.NET Core Identity (int primary keys) |
| Mapping | Mapster 7.4 |
| Email | MailKitSimplified.Sender |
| Payments | Stripe.net |
| Scaffolding | Microsoft.VisualStudio.Web.CodeGeneration.Design |
| Frontend | Razor Views, HTML5, CSS3, JavaScript, Bootstrap |

---

## 📂 Project Structure

```
eHoteloo/
├── Areas/
│   ├── Admin/                # Admin panel
│   │   ├── Controllers/      # Dashboard, Rooms, Bookings, Payments, Services, Users
│   │   └── Views/
│   ├── Staff/                # Staff panel
│   │   └── Controllers/      # Services
│   └── User/                 # Customer panel
│       └── Controllers/      # Home, Services
├── Controllers/
│   ├── HomeController.cs     # Room listing, details, booking, confirmation
│   └── AccountController.cs  # Register, login, email confirm, password reset
├── Data/
│   ├── ApplicationDbContext.cs
│   └── Configurations/       # EF Core fluent configurations
├── Migrations/               # EF Core database migrations
├── Models/                   # Domain entities
│   ├── User.cs               # Custom identity user
│   ├── Room.cs               # Room + RoomType enum
│   ├── Booking.cs            # Booking + BookingStatus enum
│   ├── Payment.cs            # Payment + enums
│   ├── Services.cs           # Hotel service requests
│   ├── RoomImage.cs
│   └── ErrorViewModel.cs
├── VM/                       # ViewModels (Mapster DTOs)
│   ├── RegisterViewModel.cs
│   ├── RoomListViewModel.cs
│   ├── RoomFilterViewModel.cs
│   ├── CreateRoomVM.cs / EditRoomVM.cs
│   ├── UserProfileVM.cs
│   └── ServiceRequestViewModel.cs
├── Services/
│   └── EmailSender.cs        # Email service (confirmation, reset password)
├── wwwroot/                  # Static files (css, js, images, lib)
├── appsettings.json
├── Program.cs                # App bootstrap, Identity config, role seeding
└── eHoteloo.csproj
```

---

## ⚡ Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- Visual Studio 2022+ **or** VS Code + C# Dev Kit

### 1. Clone the repository
```bash
git clone https://github.com/Than0s2007/eHoteloo-ASP.NET-Core.git
cd eHoteloo-ASP.NET-Core/eHoteloo
```

### 2. Configure the database connection
Edit `appsettings.json` and update the `DefaultConnection` string to match your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=eHotelooDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 3. Apply migrations & seed the database
The app seeds the default roles (`Admin`, `User`, `Staff`) on startup.

```bash
# From the eHoteloo project folder
dotnet ef database update
```

> If you need a new migration after model changes:
> ```bash
> dotnet ef migrations add MigrationName
> dotnet ef database update
> ```

### 4. Configure email (optional but recommended)
For email confirmation and password reset to work, configure your SMTP sender in `Services/EmailSender.cs` (MailKitSimplified) and add mail settings to `appsettings.json` / user secrets.

### 5. Run the application
```bash
dotnet run
```
Then open https://localhost:5001 (or the port shown in the console).

---

## 🔐 Default Roles & Access

| Role | Can do |
|------|--------|
| **Admin** | Full access to Admin area: users, rooms, bookings, payments, services, dashboard |
| **Staff** | Manage service requests (Staff area) |
| **User** | Browse rooms, book rooms, view own bookings, request services, manage profile |

Roles are seeded automatically on first run. Create an admin user by registering normally, then promoting the user to the `Admin` role via SQL or the Identity management UI.

---

## 🗺️ Key Routes

| Area | Route | Description |
|------|-------|-------------|
| Public | `/` | Home — room listing with filters |
| Public | `/Home/Details/{id}` | Room details page |
| Public | `/Account/Register` | User registration |
| Public | `/Account/Login` | Login |
| Public | `/Account/ForgotPassword` | Password reset request |
| User | `/User/Home` | User dashboard |
| User | `/User/Services` | Request / view services |
| Staff | `/Staff/Services` | Manage service requests |
| Admin | `/Admin/` | Admin dashboard |
| Admin | `/Admin/Rooms` | Manage rooms |
| Admin | `/Admin/Bookings` | Manage bookings |
| Admin | `/Admin/Users` | Manage users |
| Admin | `/Admin/Payments` | Manage payments |
| Admin | `/Admin/Services` | Manage all services |

---

## 🧩 Core Entities

- **User** — extended identity user with balance, identity photo, verification flags
- **Room** — room number, type (Single/Double/Suite), price per day, images, active flag
- **RoomImage** — photos for each room
- **Booking** — check-in/out dates, total price, status, booking number, linked payments & services
- **Payment** — amount, method (Cash/Online), status, linked to booking
- **Service** — title, description, price, status (Pending/In Progress/Completed/Cancelled)

---

## 🤝 Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Commit your changes: `git commit -m "Add some feature"`
4. Push to the branch: `git push origin feature/your-feature-name`
5. Open a Pull Request

Please follow the existing coding style and ensure your changes build successfully.

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

## 🙌 Acknowledgments

- Built by **Than0s2007**  
- Original repository: [Than0s2007/eHoteloo-ASP.NET-Core](https://github.com/Than0s2007/eHoteloo-ASP.NET-Core)

---

<div align="center">
  <b>⭐ Star this repo if you find it useful!</b>
</div>
