# Zenkoi Backend

A comprehensive backend API for managing a Koi fish farm, built with .NET 8.0. This system handles breeding processes, fish management, orders, payments, water parameter monitoring, and farm operations.

## Features

- **Koi Fish Management**: Track and manage Koi fish with AI-based re-identification
- **Breeding Process Management**: Monitor egg batches, incubation, and fry fish
- **Order & Payment Processing**: Integrated with PayOS and VnPay payment gateways
- **Farm Management**: Manage ponds, areas, and farm operations
- **Water Parameter Monitoring**: Track water quality with alerts and thresholds
- **Work Schedule Management**: Assign staff and manage work schedules
- **Customer Management**: Handle customer accounts, addresses, and favorites
- **Dashboard Analytics**: Farm and sales dashboards with insights
- **Real-time Alerts**: WebSocket support for real-time notifications
- **Image Upload**: Cloudinary integration for image storage

## Architecture

The project follows a clean architecture pattern with three main layers:

- **Zenkoi.API**: Web API layer with controllers, middlewares, and configuration
- **Zenkoi.BLL**: Business logic layer with services, DTOs, and helpers
- **Zenkoi.DAL**: Data access layer with entities, configurations, and migrations

## Prerequisites

- .NET 8.0 SDK
- SQL Server (local or remote)
- Docker (optional, for containerized deployment)

## Setup

### 1. Clone the repository

```bash
git clone <repository-url>
cd zenkoi-backend
```

### 2. Configure Database

Update the connection string in `Zenkoi.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ZenKoiDB": "Server=(local);database=ZenkoiDB;uid=sa;pwd=your_password;Trust Server Certificate=True;"
  }
}
```

### 3. Configure Application Settings

Update the following sections in `Zenkoi.API/appsettings.json`:

- **JWT**: Authentication settings
- **Cloudinary**: Image storage credentials
- **PayOS**: Payment gateway credentials
- **VnPayConfiguration**: Payment gateway settings
- **EmailConfiguration**: SMTP settings for email
- **MapConfiguration**: Map API key for distance calculations
- **Cors**: Allowed origins for CORS policy

### 4. Run Database Migrations

```bash
cd Zenkoi.API
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run --project Zenkoi.API
```

The API will be available at:

- Development: `https://localhost:7087` or `http://localhost:5000`
- Swagger UI: `https://localhost:7087/swagger`

## Docker Deployment

### Build the Docker image

```bash
docker build -t zenkoi-backend .
```

### Run the container

```bash
docker run -p 10000:10000 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__ZenKoiDB="your_connection_string" \
  zenkoi-backend
```

## Project Structure

```
zenkoi-backend/
├── Zenkoi.API/              # Web API layer
│   ├── Controllers/         # API endpoints
│   ├── Middlewares/         # Custom middlewares
│   ├── ConfigExtensions/    # Service configuration
│   └── appsettings.json     # Configuration file
├── Zenkoi.BLL/              # Business logic layer
│   ├── Services/            # Business services
│   ├── DTOs/                # Data transfer objects
│   └── Helpers/             # Utility helpers
├── Zenkoi.DAL/              # Data access layer
│   ├── Entities/            # Domain entities
│   ├── Configurations/      # EF Core configurations
│   ├── Migrations/          # Database migrations
│   └── Repositories/        # Data repositories
└── Dockerfile               # Docker configuration
```

## Authentication

The API uses JWT Bearer authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your_jwt_token>
```

## WebSocket Endpoints

- **Alerts**: `/api/ws/alerts` - Real-time water parameter alerts

## Technologies Used

- **.NET 8.0**: Framework
- **Entity Framework Core 9.0.9**: ORM
- **SQL Server**: Database
- **JWT Bearer**: Authentication
- **Swagger/OpenAPI**: API documentation
- **PayOS & VnPay**: Payment gateways
- **Cloudinary**: Image storage
- **EPPlus**: Excel file processing
- **WebSocket**: Real-time communication

## API Documentation

Once the application is running, visit `/swagger` to access the interactive API documentation.

## Background Services

The application includes several background services:

- **OrderCompletionBackgroundService**: Handles order completion logic
- **WorkScheduleStatusBackgroundService**: Updates work schedule statuses
- **PendingOrderCleanupService**: Cleans up pending orders

## Environment Variables

For production deployment, configure these environment variables:

- `ASPNETCORE_ENVIRONMENT`: Environment (Development/Production)
- `ConnectionStrings__ZenKoiDB`: Database connection string
- `PORT`: Port number (default: 10000)

## Key Features by Module

### Koi Fish Management

- Fish registration and tracking
- AI-based re-identification
- Pattern and variety management
- Favorite fish tracking

### Breeding Management

- Egg batch tracking
- Incubation daily records
- Fry fish management
- Survival rate tracking

### Order Management

- Order creation and processing
- Payment integration (PayOS, VnPay)
- Shipping management
- Order status tracking

### Farm Operations

- Pond management
- Area management
- Water parameter monitoring
- Incident tracking
- Task templates and work schedules

### Customer Management

- Customer accounts
- Address management
- Shopping cart
- Order history

## Contributing

1. Create a feature branch
2. Make your changes
3. Test thoroughly
4. Submit a pull request
