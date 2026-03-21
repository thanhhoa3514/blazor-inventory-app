# Inventory Management (Blazor Hosted .NET 9, SQL Server)

## Prerequisites

- .NET SDK 9.0+
- SQL Server (local or Docker)
- Optional Docker Desktop

## SQL Server setup (Docker)

1. Create `.env` in repo root:
   - `SA_PASSWORD=YourStrong@Passw0rd`
2. Start SQL Server:
   - `docker compose up -d`
3. Confirm connection string in `Server/appsettings.Development.json` matches your password and port (`localhost,1440`).

## Database migrations

Run from repo root:

```powershell
dotnet ef database update --project Server\MyApp.Server.csproj --startup-project Server\MyApp.Server.csproj
```

## Run the app

```powershell
dotnet restore MyApp.sln
dotnet run --project Server\MyApp.Server.csproj
```

Open the URL shown by the server (typically `https://localhost:7006`).

## Test commands

```powershell
dotnet test
```

## Common troubleshooting

- `Cannot connect to SQL Server`:
  - Ensure Docker SQL container is running: `docker ps`
  - Ensure `SA_PASSWORD` and `DefaultConnection` match.
- `Login failed for user 'sa'`:
  - Password in `.env` and `appsettings*.json` are different; align them.
- `Port already in use`:
  - Change docker host mapping from `1440:1433` and update connection string.
- `NuGet/network restore errors`:
  - Check internet/proxy and retry `dotnet restore`.
