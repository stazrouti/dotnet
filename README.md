# Bibliotheque

Bibliotheque is an ASP.NET Core MVC application built with .NET 10, Entity Framework Core, and ASP.NET Identity. It uses SQL Server as its database and is structured around a small library management domain.

## What the project does

The application provides the foundation for managing:

- Students (`Etudiant`)
- Books (`Livre`)
- Borrowing records (`Emprunt`)
- Keywords or tags for books (`Motcle`)
- Visits by students (`Visite`)
- User authentication with ASP.NET Identity (`User`)

The current web app includes:

- A default home page and privacy page
- Login, register, and logout actions in `AccountController`
- Entity Framework Core models and migrations for the library database

## How it works

The app starts in `Program.cs`, where it:

1. Registers `BibliothequeContext` with SQL Server.
2. Configures ASP.NET Identity for local authentication.
3. Enables MVC controllers, Razor views, and Razor Pages.
4. Sets the default route to `Home/Index`.

The database model is defined in `Models/BibliothequeContext.cs` and the entity classes under `Models/`. Migrations in `Migrations/` describe how the schema is created and updated.

### Main data model

- `Livre` represents a book, identified by `Numinventaire`.
- `Etudiant` represents a student, identified by `Cin`.
- `Emprunt` links a student to a borrowed book.
- `Motcle` links keywords to a book.
- `Visite` stores student visit records.
- `User` extends ASP.NET Identity for application users.

## Requirements

- .NET 10 SDK
- SQL Server installed locally or reachable from your machine
- A database named `bibliotheque`, or a connection string that points to your database
- Entity Framework CLI tool (`dotnet-ef`)

The default connection string is defined in `appsettings.json`.

## Run locally

### Using the command line

From the project folder:

```bash
dotnet restore
dotnet tool install --global dotnet-ef
dotnet ef database update
dotnet run
```

The app will usually be available at:

- `https://localhost:7245`
- `http://localhost:5047`

These URLs come from `Properties/launchSettings.json`.

### Using Visual Studio

1. Open `Bibliotheque.sln`.
2. Make sure the SQL Server connection in `appsettings.json` is valid.
3. Restore NuGet packages.
4. Apply migrations if the database is not created yet.
5. Run the project.

## Database setup

The app uses this default connection string:

`Data Source=.;Initial Catalog=bibliotheque;Integrated Security=True;Encrypt=True;Trust Server Certificate=True`

If your SQL Server instance is different, update `ConnectionStrings:DefaultConnection` in `appsettings.json` before running the project.

If the database does not exist yet, create it with Entity Framework migrations:

```bash
dotnet ef database update
```

## Project structure

- `Controllers/` contains MVC controllers.
- `Models/` contains the EF Core entities and view models.
- `Views/` contains Razor views for the UI.
- `Migrations/` contains database migration history.
- `wwwroot/` contains static assets such as CSS, JavaScript, and images.

## Notes

- `bin/`, `obj/`, `.vs/`, and local database files are ignored by Git.
- The project is set up for development on Windows with SQL Server.
- If you change the schema, create a new migration and update the database.
