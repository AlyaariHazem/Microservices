# Getting Started Guide

This guide will help you set up and run the Microservices project on your local machine.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Running with Docker](#running-with-docker)
- [Running Services Individually](#running-services-individually)
- [Testing the APIs](#testing-the-apis)
- [Troubleshooting](#troubleshooting)

## Prerequisites

Before you begin, ensure you have the following installed:

### Required Software

1. **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**
   ```bash
   # Verify installation
   dotnet --version
   # Should show 8.0.x or higher
   ```

2. **[Docker Desktop](https://www.docker.com/products/docker-desktop)**
   ```bash
   # Verify installation
   docker --version
   docker-compose --version
   ```

3. **[Git](https://git-scm.com/)**
   ```bash
   # Verify installation
   git --version
   ```

### Optional Tools

- **[Visual Studio 2022](https://visualstudio.microsoft.com/)** or **[Visual Studio Code](https://code.visualstudio.com/)**
- **[Postman](https://www.postman.com/)** or any API testing tool
- **[pgAdmin](https://www.pgadmin.org/)** for PostgreSQL management
- **[Redis Insight](https://redis.io/insight/)** for Redis management

## Installation

### Step 1: Clone the Repository

```bash
git clone https://github.com/AlyaariHazem/Microservices.git
cd Microservices
```

### Step 2: Verify Project Structure

```bash
# List the main directories
ls -la

# You should see:
# - src/Services/
# - Basket.API/
# - BuildingBlocks/
# - docker-compose.yml
# - README.md
```

## Running with Docker

This is the **recommended approach** for running all services together.

### Step 1: Build and Start All Services

```bash
# Build and start all containers
docker-compose up -d

# View logs
docker-compose logs -f

# Or view logs for a specific service
docker-compose logs -f catalog.api
```

### Step 2: Verify Services are Running

```bash
# Check running containers
docker ps

# You should see:
# - catalogapi
# - basketapi
# - discountgrpc
# - catalogdb
# - basketdb
# - distributedcache
```

### Step 3: Access the Services

Open your browser and navigate to:

- **Catalog API Swagger**: http://localhost:5000/swagger
- **Basket API Swagger**: http://localhost:6001/swagger

### Step 4: Stop Services

```bash
# Stop all containers
docker-compose down

# Stop and remove volumes (cleans up databases)
docker-compose down -v
```

## Running Services Individually

If you prefer to run services without Docker:

### Step 1: Start Required Databases

You'll need PostgreSQL and Redis running. You can use Docker for just the databases:

```bash
# Start only the databases
docker-compose up -d catalogdb basketdb distributedcache
```

Or install them locally:
- **PostgreSQL**: Create databases `CatalogDb` and `BasketDb`
- **Redis**: Run on default port 6379

### Step 2: Run Catalog API

```bash
cd src/Services/Catalog/Catalog.API
dotnet restore
dotnet run
```

Access at: http://localhost:5000/swagger

### Step 3: Run Basket API

```bash
cd Basket.API
dotnet restore
dotnet run
```

Access at: http://localhost:6001/swagger

### Step 4: Run Discount gRPC Service

```bash
cd src/Services/Discount/Discount.Grpc
dotnet restore
dotnet run
```

Listening on: http://localhost:6002

### Step 5: Run Ordering API (When Available)

```bash
cd src/Services/Ordering/Ordering.API
dotnet restore
dotnet run
```

## Testing the APIs

### Using Swagger UI

1. **Navigate to Swagger UI** for any service
2. **Expand an endpoint** (e.g., POST /products)
3. **Click "Try it out"**
4. **Enter the request body**
5. **Click "Execute"**
6. **View the response**

### Example: Create a Product (Catalog API)

1. Open http://localhost:5000/swagger
2. Find **POST /products** endpoint
3. Click "Try it out"
4. Use this sample request:

```json
{
  "name": "iPhone 14",
  "category": ["Electronics", "Smartphones"],
  "description": "Latest iPhone model",
  "imageFile": "iphone14.png",
  "price": 999.99
}
```

5. Click "Execute"
6. You should get a 201 Created response

### Example: Get All Products

1. Find **GET /products** endpoint
2. Click "Try it out"
3. Click "Execute"
4. You should see a list of products

### Example: Create a Basket (Basket API)

1. Open http://localhost:6001/swagger
2. Find **POST /basket** endpoint
3. Click "Try it out"
4. Use this sample request:

```json
{
  "userName": "john_doe",
  "items": [
    {
      "quantity": 2,
      "color": "Black",
      "price": 999.99,
      "productId": "5334c996-8457-4cf0-815c-ed2b77c4ff61",
      "productName": "iPhone 14"
    }
  ]
}
```

5. Click "Execute"

### Using cURL

```bash
# Get all products
curl -X GET http://localhost:5000/products

# Create a product
curl -X POST http://localhost:5000/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "iPhone 14",
    "category": ["Electronics"],
    "description": "Latest iPhone",
    "imageFile": "iphone14.png",
    "price": 999.99
  }'

# Get a basket
curl -X GET http://localhost:6001/basket/john_doe
```

### Using Postman

1. Import the API collection (if available)
2. Or create requests manually using the endpoints from Swagger
3. Set the base URL to the service URL
4. Add necessary headers (Content-Type: application/json)
5. Execute requests

## Exploring the Database

### PostgreSQL (Catalog & Basket)

**Using pgAdmin or any PostgreSQL client:**

**Catalog Database:**
- Host: localhost
- Port: 5432
- Database: CatalogDb
- User: postgres
- Password: postgres

**Basket Database:**
- Host: localhost
- Port: 5433
- Database: BasketDb
- User: postgres
- Password: postgres

**Using psql:**

```bash
# Connect to Catalog database
docker exec -it catalogdb psql -U postgres -d CatalogDb

# List tables
\dt

# Query products (Marten stores as JSON)
SELECT * FROM mt_doc_product;

# Exit
\q
```

### Redis (Basket Cache)

**Using Redis CLI:**

```bash
# Connect to Redis
docker exec -it distributedcache redis-cli

# List all keys
KEYS *

# Get a basket (replace username)
GET basket:john_doe

# Exit
exit
```

**Using Redis Insight:**
- Connect to localhost:6379
- Browse keys and values visually

## Troubleshooting

### Common Issues

#### 1. Port Already in Use

**Error:** "Port 5000 is already allocated"

**Solution:**
```bash
# Find what's using the port
lsof -i :5000  # macOS/Linux
netstat -ano | findstr :5000  # Windows

# Kill the process or change the port in docker-compose.override.yml
```

#### 2. Docker Containers Not Starting

**Solution:**
```bash
# Check container logs
docker-compose logs [service-name]

# Rebuild images
docker-compose build --no-cache

# Start fresh
docker-compose down -v
docker-compose up -d
```

#### 3. Database Connection Errors

**Error:** "Could not connect to database"

**Solution:**
- Ensure database containers are running: `docker ps`
- Check connection strings in appsettings.json
- Verify database is initialized: `docker logs catalogdb`

#### 4. .NET Build Errors

**Solution:**
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

#### 5. Redis Connection Issues

**Solution:**
```bash
# Verify Redis is running
docker ps | grep redis

# Test Redis connection
docker exec -it distributedcache redis-cli ping
# Should return: PONG
```

### Getting Help

If you encounter issues:

1. Check the [Issues](https://github.com/AlyaariHazem/Microservices/issues) page
2. Review the [Architecture Documentation](docs/ARCHITECTURE.md)
3. Open a new issue with:
   - Clear description of the problem
   - Steps to reproduce
   - Error messages or logs
   - Your environment (OS, .NET version, Docker version)

## Next Steps

Now that you have the services running:

1. **Explore the APIs** using Swagger UI
2. **Review the code** to understand the implementation
3. **Read the [Architecture Documentation](docs/ARCHITECTURE.md)**
4. **Try making changes** to a service
5. **Add new features** and submit a PR

Happy coding! 🚀
