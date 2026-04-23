# Madina al Attaa (GivingChampion)

A gamified charitable platform built with .NET 10.0 that connects donors, volunteers, and partners to make a positive impact through donations, service requests, and volunteer missions.

## 🌟 Features

- **User Management**: ASP.NET Identity with JWT Bearer authentication and Google OAuth integration
- **Gamification System**: 
  - Avatar customization (AI-generated avatars)
  - Level progression system
  - Badge achievements
  - Geo-quests and missions
  - User reviews and ratings
- **Donation Management**: 
  - Donation requests and orders
  - Child sponsorship tracking
- **Volunteer Coordination**:
  - Service requests
  - Volunteer orders
  - Certificate generation
- **Partner Management**: Partner organization profiles and services
- **Location Services**: Geographic location tracking for missions and quests
- **API Documentation**: Scalar API reference with OpenAPI specification

## 🏗️ Architecture

This project follows Clean Architecture principles with the following layers:

- **GivingChampion.API**: Web API layer with controllers and authentication
- **GivingChampion.Application**: Business logic, services, and DTOs
- **GivingChampion.Domain**: Domain entities and DbContext
- **GivingChampion.Infrastructure**: Data persistence with Entity Framework Core
- **GivingChampion.Common**: Shared DTOs and common utilities
- **GivingChampion.Migrator**: Database migration tool
- **GivingChampion.Seeder**: Database seeding for development
- **GivingChampion.AppHost**: .NET Aspire orchestration for local development

## 📋 Prerequisites

- **.NET 10.0 SDK** - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
- **.NET Aspire CLI** - Install with: `dotnet workload install aspire`
- **Docker Desktop** - Required for PostgreSQL container in development
- **PostgreSQL** - (Optional) For production database
- **Google OAuth Credentials** - For Google authentication (optional)

## 🚀 Installation

### Clone the Repository

```bash
git clone https://github.com/your-username/madina-al-ataa.git
cd madina-al-ataa
```

### Restore Dependencies

```bash
dotnet restore
```

### Configure Secrets

Set up user secrets for sensitive configuration:

```bash
cd GivingChampion.API
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your-super-secret-jwt-key-min-32-chars"
dotnet user-secrets set "Authentication:Google:ClientSecret" "your-google-client-secret"
```

Or configure environment variables:

```bash
set Jwt__Key=your-super-secret-jwt-key-min-32-chars
set Authentication__Google__ClientSecret=your-google-client-secret
```

## 💻 Development

### Using .NET Aspire (Recommended)

The project uses .NET Aspire for orchestration, which automatically manages PostgreSQL, migrations, and seeding.

1. **Navigate to the AppHost directory:**
   ```bash
   cd GivingChampion\GivingChampion.AppHost
   ```

2. **Run the Aspire AppHost:**
   ```bash
   dotnet run
   ```

This will:
- Start PostgreSQL in a Docker container
- Run database migrations automatically
- Seed the database with initial data
- Start the API on `http://localhost:5000`
- Launch the Aspire Dashboard on `http://localhost:8080`

### Manual Development Setup

If you prefer to run services manually:

1. **Start PostgreSQL:**
   ```bash
   docker run --name givingchampion-db -e POSTGRES_PASSWORD=postgres -e POSTGRES_USER=postgres -p 5432:5432 -d postgres
   ```

2. **Run Migrations:**
   ```bash
   cd GivingChampion.Migrator
   dotnet run
   ```

3. **Seed the Database:**
   ```bash
   cd GivingChampion.Seeder
   dotnet run
   ```

4. **Run the API:**
   ```bash
   cd GivingChampion.API
   dotnet run
   ```

The API will be available at `https://localhost:5000` (or the port specified in your launch settings).

## 📚 API Documentation

Once the API is running, access the interactive API documentation:

- **Scalar API Reference**: `https://localhost:5000/scalar/v1`
- **OpenAPI Spec**: `https://localhost:5000/openapi/v1.json`

## 🔧 Configuration

### Connection Strings

Update the connection string in `appsettings.json` or use environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=givingchampion;Username=postgres;Password=postgres"
  }
}
```

### CORS Configuration

Configure allowed origins in `appsettings.json`:

```json
{
  "Cors": {
    "AllowedOrigins": ["http://localhost:5173"]
  }
}
```

### Google Authentication

To enable Google authentication:

1. Create a project in [Google Cloud Console](https://console.cloud.google.com)
2. Enable Google+ API
3. Create OAuth 2.0 credentials
4. Add your callback URL: `https://yourdomain.com/signin-google`
5. Update `appsettings.json` or user secrets with your credentials

## 🧪 Testing

Run tests (if available):

```bash
dotnet test
```

## 📦 Building for Production

```bash
dotnet publish -c Release -o ./publish
```

## 🚢 Production Deployment with Docker Compose

The project uses .NET Aspire to generate Docker Compose configurations for production deployment.

### Generate Docker Compose Files

1. **Navigate to the AppHost directory:**
   ```bash
   cd GivingChampion\GivingChampion.AppHost
   ```

2. **Generate Docker Compose manifest:**
   ```bash
   aspire deploy
   ```

   This will generate the following files in the `aspire-output` directory at the root of the project:
   - `.env` - Template environment file (committed to Git)
   - `.env.Production` - Production environment file (contains actual secrets, not committed)
   - `docker-compose.yml` - Docker Compose configuration (already configured)

### Configure Environment Variables

The `.env` file in `aspire-output` is a template that will be committed to GitHub. Copy it and fill in your production secrets:

```bash
cd aspire-output
cp .env .env.Production
```

Edit `.env.Production` with your actual production values:

```env
# Database credentials
POSTGRES_USER=your_production_db_user
POSTGRES_PASSWORD=your_secure_db_password

# JWT Configuration
Jwt__Key=your-super-secret-jwt-key-min-32-chars
Jwt__Issuer=GivingChampion
Jwt__Audience=GivingChampion.Client
Jwt__AccessTokenMinutes=60

# Google Authentication
Authentication__Google__ClientId=your-google-client-id
Authentication__Google__ClientSecret=your-google-client-secret

# Connection String (if using external PostgreSQL)
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=givingchampion;Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}
```

**Important**: `.env.Production` contains sensitive secrets and should never be committed to version control. Add it to `.gitignore` if not already present.

### Deploy with Docker Compose

1. **Navigate to the aspire-output directory:**
   ```bash
   cd aspire-output
   ```

2. **Start all services:**
   ```bash
   docker-compose --env-file .env.Production up -d
   ```

   This will start:
   - PostgreSQL database
   - Database migrator
   - Database seeder
   - API service

3. **View logs:**
   ```bash
   docker-compose --env-file .env.Production logs -f
   ```

4. **Stop services:**
   ```bash
   docker-compose --env-file .env.Production down
   ```

### Production Considerations

- **Database Persistence**: The Docker Compose configuration includes a data volume for PostgreSQL to persist data across container restarts
- **External Database**: For production, consider using a managed PostgreSQL service (e.g., Azure Database for PostgreSQL, AWS RDS) and update the connection string accordingly
- **SSL/TLS**: Configure HTTPS termination using a reverse proxy (e.g., Nginx, Traefik) in front of the API
- **Environment Variables**: Never commit `.env` files to version control. Use secret management solutions for production (e.g., Azure Key Vault, AWS Secrets Manager)
- **Health Checks**: The API includes health check endpoints at `/health` for monitoring
- **Scaling**: For horizontal scaling, use a container orchestration platform like Kubernetes or Azure Container Apps

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License.

## 📞 Contact

For questions or support, please open an issue on GitHub.