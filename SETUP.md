# DataSet Explorer - Setup Guide

## About

The DataSet Explorer (DSE) tool supports annotators during the code smell annotation procedure. It helps ML researchers and annotators build high-quality datasets for training ML code smell detection models.

This tool speeds up and eases manual code smell annotation by providing various functionalities to support the annotation process, helping to address issues like inconsistent annotations, small dataset size, and poor smell coverage.

**Useful Resources:**
- [Back-end source code](https://github.com/Clean-CaDET/dataset-explorer) - This repository
- [Front-end source code](https://github.com/Clean-CaDET/platform-explorer-ui-web) - Web UI repository
- [Documentation](https://github.com/Clean-CaDET/dataset-explorer/wiki) - Wiki pages with design and features

---

## Prerequisites

Tested on Windows 10 and 11

### Required Software
- [Git for Windows](https://git-scm.com/download/win) - To clone repositories
- [Docker Desktop for Windows](https://docs.docker.com/desktop/install/windows-install/) - To run the application

### Optional Software (for active development)
- [.NET 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0) - Only needed if you plan to modify database models and prefer creating migrations locally
- EF Core CLI tools - Install with: `dotnet tool install --global dotnet-ef`

## Setup Steps

### 1. Clone the Repositories

You need to clone both the backend and frontend repositories:

```bash
# Clone the backend repository
git clone https://github.com/Clean-CaDET/dataset-explorer.git

# Clone the frontend repository
git clone https://github.com/Clean-CaDET/platform-explorer-ui-web.git
```

### 2. Configure Environment Variables

Navigate to the backend repository and create a `.env` file:

```bash
cd dataset-explorer
```

Copy the example file and edit it:

```bash
cp .env.example .env
```

Edit the `.env` file with your settings:

```env
# Database Configuration
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_DB=data-set-explorer-db
DATABASE_HOST=database
DATABASE_PORT=5432

# Git Credentials
GIT_USER=your-github-username
GIT_TOKEN=your-github-token

# Frontend Port (the port you'll access the app on)
FRONTEND_PORT=4200

# Frontend Source Path
# Adjust this path to where you cloned the frontend repository (Set this to the absolute path where you cloned the frontend repository OR use relative path if frontend is in parent directory)
# Use forward slashes (/) even on Windows
# Example: D:/projects/platform-explorer-ui-web
FRONTEND_PATH=../platform-explorer-ui-web

# User Downloads folder for exports
# This is where exported analysis files will be saved
# Use forward slashes (/) even on Windows
# Example: C:/Users/YourUsername/Downloads
DOWNLOADS_PATH=C:/Users/YourUsername/Downloads
```

### 3. Start the Application

From the `dataset-explorer` directory, run:

```bash
docker compose up -d --build
```

This will:
- Build the backend and frontend Docker images
- Start PostgreSQL database
- Automatically apply database migrations
- Start all services

**First-time setup:** The build process may take 5-10 minutes as Docker downloads base images and builds the application.

### 4. Verify the Application is Running

Check that all containers are running:

```bash
docker ps
```

You should see three containers:
- `dataset-explorer-database` (PostgreSQL)
- `dataset-explorer-backend` (ASP.NET Core API)
- `dataset-explorer-frontend` (Angular + Nginx)

### 5. Access the Application

Open your browser and navigate to:

```
http://localhost:4200
```

(Or the port you configured in `FRONTEND_PORT`)

### 6. Check Logs (if needed)

If you encounter any issues, check the logs:

```bash
# View all logs
docker compose logs

# View specific service logs
docker compose logs backend
docker compose logs frontend
docker compose logs database

# Follow logs in real-time
docker compose logs -f backend
```

## Database Migrations

**Good news:** Database migrations are applied automatically when the backend starts!

The application uses Entity Framework Core migrations, which are automatically applied on startup. You don't need to run any manual migration commands.

### Creating New Migrations

If you modify database models (add/remove/change entities), you need to create a new migration. You have two options:

#### Option 1: With .NET SDK Installed (Recommended for active development)

**Prerequisites:**
- [.NET 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
- EF Core CLI tools: `dotnet tool install --global dotnet-ef`

```bash
# Navigate to the backend directory
cd DataSetExplorer

# Create a new migration
dotnet ef migrations add YourMigrationName

# The migration will be automatically applied next time you restart the backend
docker compose restart backend
```

#### Option 2: Without .NET SDK (Using Docker)

If you don't have .NET SDK installed, you can create migrations inside the running backend container, but you'll need to copy the files out:

```bash
# Make sure the backend container is running
docker compose up -d backend

# Create a new migration inside the container
docker exec dataset-explorer-backend dotnet ef migrations add YourMigrationName --project /app

# Copy the new migration files from container to local filesystem
docker cp dataset-explorer-backend:/app/Migrations/. ./DataSetExplorer/Migrations/

# Restart the backend to apply the migration
docker compose restart backend
```

**Note:** The `docker cp` command copies all migration files from the container to your local `Migrations/` folder. Make sure to commit the new migration files to git.

**Important:** Since source code is not mounted as a volume, changes made inside the container (including new migration files) require the `docker cp` command to appear in your local filesystem.

## Stopping the Application

To stop all containers:

```bash
docker compose down
```

To stop and remove all data (including database):

```bash
docker compose down -v
```

## Rebuild After Code Changes

If you make changes to the code:

```bash
# Rebuild specific service
docker compose up -d --build backend
docker compose up -d --build frontend

# Or rebuild everything
docker compose up -d --build
```

## Complete Cleanup

To completely remove all Docker resources:

```bash
# Stop and remove containers, networks, and volumes
docker compose down -v

# Remove all unused Docker resources
docker system prune -a

# Remove all unused volumes
docker volume prune
```

## Troubleshooting

### Port Already in Use
If port 4200 is already in use, change `FRONTEND_PORT` in `.env` to another port (e.g., 8080).

### Cannot Access Application
1. Verify all containers are running: `docker ps`
2. Check backend logs: `docker compose logs backend`
3. Verify the database is healthy: `docker compose ps`

### Database Connection Issues
1. Ensure the database container is healthy: `docker compose ps`
2. Check database logs: `docker compose logs database`
3. Verify credentials in `.env` match the database configuration

### Frontend Build Fails
1. Verify `FRONTEND_PATH` in `.env` points to the correct frontend repository location
2. Ensure the frontend repository is cloned
3. Check frontend logs: `docker compose logs frontend`

### Export Files Not Appearing
1. Verify `DOWNLOADS_PATH` in `.env` is correct
2. Ensure Docker Desktop has access to your Downloads folder (Settings → Resources → File Sharing)

## Development Workflow

1. Make code changes in your IDE
2. Rebuild the affected service:
   ```bash
   docker compose up -d --build backend  # for backend changes
   docker compose up -d --build frontend # for frontend changes
   ```
3. The application will restart automatically with your changes

## Notes

- Database data persists between container restarts (stored in Docker volume)
- Cloned Git repositories are stored in a Docker volume
- Exported files are saved to your Downloads folder
- All migrations are applied automatically on backend startup
