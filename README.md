# Bibliotheque

Bibliotheque is an ASP.NET Core MVC application built with .NET 10, Entity Framework Core, ASP.NET Identity, and SQL Server.

It manages books and students with role-based access (`Admin` and `User`) and includes reservation and review workflows.

## Features implemented

### Authentication and users

- Register, login, logout
- Password change and profile page
- Automatic first-user promotion to `Admin`
- Role-based authorization for admin/student features

### Books (`Livres`)

- List, details, create, edit, delete
- Search by inventory number, title, author, or ISBN
- Pagination on books list: **10 books per page**

### Reservations (`Emprunts`)

- Student reservation flow from book details
- Reservation date range (`StartDate`, `EndDate`)
- Arrival-date guard: reservation start must be on/after `Datearrivage`
- Availability conflict checks (no overlap for active reservations)
- Reservation quota per student (max 3 active/upcoming)
- Reservation cancellation by the owning student

### Student dashboard

- "Mes reservations" dashboard for verified students
- Current reservations + history
- Status aggregation (`Active`, `Reserved`, `Returned`, `Cancelled`)
- Alerts and counters for due dates
- Home dashboard summary cards (today, due today, due this week, quota)

### Reviews and ratings (`Avis`)

- Reviews shown on book details page
- Create review only for eligible students (verified + returned reservation history)
- One review per student per book
- Update/delete review by the review owner only
- Average rating and total review count per book

## Architecture

### Application layers

- `Controllers/`: MVC endpoints and authorization checks
- `Services/`: business logic and database operations
- `Models/`: EF entities and view models
- `Views/`: Razor UI

### Core entities

- `Livre`: Book (primary key: `Numinventaire`)
- `Etudiant`: Student (primary key: `Cin`)
- `Emprunt`: Reservation/borrowing record
- `Avis`: Book review/rating
- `Motcle`: Keywords/tags for books
- `Visite`: Visit records
- `User`: ASP.NET Identity user extension

## Requirements

- .NET 10 SDK
- SQL Server
- `dotnet-ef` tool

## Setup and run

From project root:

```bash
dotnet restore
dotnet tool install --global dotnet-ef
dotnet ef database update
dotnet run
```

Typical local URLs (from launch settings):

- `https://localhost:7245`
- `http://localhost:5047`

## Database

Default connection string is in `appsettings.json`.

Example default value:

`Data Source=.;Initial Catalog=bibliotheque;Integrated Security=True;Encrypt=True;Trust Server Certificate=True`

If needed, update `ConnectionStrings:DefaultConnection` before running migrations.

## Project structure

- `Controllers/`
- `Services/`
- `Models/`
- `Views/`
- `Migrations/`
- `wwwroot/`

## Notes

- This project currently uses standard MVC folders (no ASP.NET Areas yet).
- If the schema changes, create and apply a migration:

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```
