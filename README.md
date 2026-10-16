<div align="center">

# 🏨 eHoteloo

### Hotel room booking & management system

Browse rooms, book instantly, pay from your wallet, and manage everything from one dashboard.

</div>

---

## 📖 Overview

**eHoteloo** is a full-stack hotel management web app built with **ASP.NET Core MVC (.NET 9)** and **Microsoft SQL Server**.

It serves three types of users, each with their own panel:

| Role | What they can do |
|------|------------------|
| 🧑 **Guest (User)** | Browse and filter rooms, book with an automatic availability check, pay from wallet balance, request hotel services |
| 🧑‍💼 **Staff** | View and process service requests (pending → in progress → completed / cancelled) |
| 🛡️ **Admin** | Manage users, rooms, bookings, payments, and services from a central dashboard |

---

## ✨ Features

### 👤 Authentication & Accounts
- Registration and login with **ASP.NET Core Identity**
- Email confirmation
- Password reset via email
- Role-based access control (Admin / Staff / User)

### 🛏️ Rooms & Bookings
- Browse and filter rooms by **type**, **price**, and **capacity**
- Booking with **automatic availability check**
- User **wallet balance** for payments

### 🛎️ Hotel Services
- Guests submit service requests
- Staff track them through: `Pending` → `In Progress` → `Completed` / `Cancelled`

### 📊 Admin Dashboard
- Full management of users, rooms, bookings, payments, and services

---


## 🧰 Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core MVC (.NET 9) |
| Language | C# |
| Database | Microsoft SQL Server |
| ORM | Entity Framework Core 9 |
| Auth | ASP.NET Core Identity |
---

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Microsoft SQL Server](https://www.microsoft.com/sql-server) (or SQL Server Express / LocalDB)
- EF Core CLI tools:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### Installation

**1. Clone the repository**
```bash
git clone https://github.com/Than0s2007/eHoteloo-ASP.NET-Core.git
cd eHoteloo-ASP.NET-Core/eHoteloo
```

**2. Configure the database** in `appsettings.json`
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=eHotelooDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**3. Configure email and Stripe**

Use [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) so you never commit real keys:

```bash
dotnet user-secrets init
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."
dotnet user-secrets set "Stripe:PublishableKey" "pk_test_..."
dotnet user-secrets set "EmailSettings:Host" "smtp.example.com"
dotnet user-secrets set "EmailSettings:Port" "587"
dotnet user-secrets set "EmailSettings:Email" "you@example.com"
dotnet user-secrets set "EmailSettings:Password" "your-app-password"
```

> ⚠️ The key names above are examples. Match them to the section names your `appsettings.json` actually uses.
> 💡 Use Stripe **test mode** keys while developing.

**4. Apply migrations**
```bash
dotnet ef database update
```

**5. Run the app**
```bash
dotnet run
```

Then open the URL printed in the terminal (usually `https://localhost:xxxx`).

---

## 🔑 Demo Accounts

> Fill this in if your project seeds default users.

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@example.com` | `********` |
| Staff | `staff@example.com` | `********` |
| User | `user@example.com` | `********` |

---

## 📂 Project Structure

```
eHoteloo/
├── Areas/
│   ├── Admin/        # Admin panel
│   ├── Staff/        # Staff panel
│   └── User/         # Guest panel
├── Controllers/      # Home & Account controllers
├── Models/           # Entities: User, Room, Booking, Payment, Service
├── VM/               # View models
├── Data/             # ApplicationDbContext & configurations
├── Services/         # Email sender
└── wwwroot/          # Static files (CSS, JS, images)
```


---


<div align="center">

Made with ❤️ using ASP.NET Core

⭐ If you like this project, give it a star!

</div>
