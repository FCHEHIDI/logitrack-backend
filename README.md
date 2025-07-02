# LogiTrack API

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
    A[Client] -->|JWT Auth| B(AuthController)
    A -->|GET, POST, DELETE| C(InventoryController)
    A -->|GET, POST, DELETE| D(OrderController)
    A -->|GET| E(HealthCheck)

    B -->|POST /api/auth/register| F1[Register]
    B -->|POST /api/auth/login| F2[Login]
    B -->|POST /api/auth/confirm-email| F3[Email Confirm]

    C -->|GET /api/inventory| G1[List Inventory]
    C -->|POST /api/inventory| G2[Add Item]
    C -->|DELETE /api/inventory/{id}| G3[Delete Item]

    D -->|GET /api/orders| H1[List Orders]
    D -->|GET /api/orders/{id}| H2[Order By Id]
    D -->|POST /api/orders| H3[Create Order]
    D -->|DELETE /api/orders/{id}| H4[Delete Order]

    E -->|GET /health| I1[Health Status]
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

© 2025 Your Name. For learning and portfolio use.
