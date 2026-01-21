# LogiTrack API

![logitrack-backend](payloadtraffic.png)

Production-ready ASP.NET Core API for inventory and order management, built as a Microsoft Backend Capstone project.

## Features
- Entity Framework Core (SQLite)
- ASP.NET Core Identity & JWT authentication
- Role-based authorization
- In-memory caching
- Optimized queries
- Health checks & global error handling
- Docker support
- Azure Pipelines CI/CD (simulation)

## API Route Overview

```mermaid
flowchart TD
    Client -->|JWT Auth| AuthController
    Client -->|GET, POST, DELETE| InventoryController
    Client -->|GET, POST, DELETE| OrderController
    Client -->|GET| HealthCheck

    AuthController -->|POST /api/auth/register| Register
    AuthController -->|POST /api/auth/login| Login
    AuthController -->|POST /api/auth/confirm-email| EmailConfirm

    InventoryController -->|GET /api/inventory| ListInventory
    InventoryController -->|POST /api/inventory| AddItem
    InventoryController -->|DELETE /api/inventory/id| DeleteItem

    OrderController -->|GET /api/orders| ListOrders
    OrderController -->|GET /api/orders/id| OrderById
    OrderController -->|POST /api/orders| CreateOrder
    OrderController -->|DELETE /api/orders/id| DeleteOrder

    HealthCheck -->|GET /health| HealthStatus
```

## Quickstart

1. Clone the repo and restore dependencies:
   ```sh
   git clone <your-repo-url>
   cd BackendCapstoneProject
   dotnet restore
   ```
2. Run locally:
   ```sh
   dotnet run --project LogiTrack/LogiTrack.csproj
   ```
3. Build and run with Docker:
   ```sh
   cd LogiTrack
   docker build -t logitrack-api .
   docker run -p 5000:5000 logitrack-api
   ```

## Documentation
- See the `docs/` folder for architecture, API docs, test plans, Docker, and CI/CD details.

---

© 2025 Fares Chehidi.
