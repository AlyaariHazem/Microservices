# Architecture Documentation

## Overview

This document provides detailed information about the microservices architecture implemented in this project.

## System Architecture

### High-Level Architecture

The system follows a microservices architecture pattern where each service is:
- **Independently deployable**
- **Has its own database** (Database per Service pattern)
- **Communicates via well-defined APIs** (REST/gRPC)
- **Containerized with Docker**

### Communication Patterns

#### Synchronous Communication
- **REST APIs**: Used for client-to-service communication
- **gRPC**: Used for service-to-service communication (e.g., Basket → Discount)

#### Asynchronous Communication (Planned)
- **Message Broker**: RabbitMQ or Azure Service Bus
- **Event-Driven**: Services publish and subscribe to domain events

## Service Details

### Catalog Service

**Responsibilities:**
- Manage product catalog
- Product CRUD operations
- Category management
- Product search and filtering

**Technology Stack:**
- ASP.NET Core 8 Minimal API
- Carter for routing
- Marten for document database and event sourcing
- PostgreSQL for persistence
- MediatR for CQRS
- FluentValidation

**Database Schema:**
```
Products Table:
- Id (UUID)
- Name (string)
- Category (list)
- Description (string)
- ImageFile (string)
- Price (decimal)
```

**API Endpoints:**
```
GET    /products              - List all products
GET    /products/{id}         - Get product by ID
GET    /products/category/{category} - Get products by category
POST   /products              - Create product
PUT    /products              - Update product
DELETE /products/{id}         - Delete product
```

### Basket Service

**Responsibilities:**
- Manage shopping carts
- Apply discounts
- Prepare basket for checkout

**Technology Stack:**
- ASP.NET Core 8 Minimal API
- Carter for routing
- PostgreSQL for persistence
- Redis for distributed caching
- gRPC client for Discount service
- MediatR for CQRS

**Database Schema:**
```
ShoppingCart Table:
- UserName (string, PK)
- Items (list of ShoppingCartItem)
- TotalPrice (decimal)

ShoppingCartItem:
- Quantity (int)
- Color (string)
- Price (decimal)
- ProductId (UUID)
- ProductName (string)
```

**API Endpoints:**
```
GET    /basket/{username}     - Get user basket
POST   /basket                - Store basket
DELETE /basket/{username}     - Delete basket
POST   /basket/checkout       - Checkout basket
```

**Integrations:**
- Calls Discount service via gRPC to get product discounts

### Discount Service

**Responsibilities:**
- Manage discount rules
- Provide discount information to other services

**Technology Stack:**
- ASP.NET Core 8 gRPC
- SQLite for lightweight storage
- Entity Framework Core

**Database Schema:**
```
Coupon Table:
- Id (int, PK)
- ProductName (string)
- Description (string)
- Amount (int)
```

**gRPC Methods:**
```
GetDiscount(ProductName)    - Get discount for product
CreateDiscount(Coupon)      - Create new discount
UpdateDiscount(Coupon)      - Update existing discount
DeleteDiscount(ProductName) - Delete discount
```

### Ordering Service

**Responsibilities:**
- Process orders
- Manage order lifecycle
- Order history

**Technology Stack:**
- ASP.NET Core 8
- Clean Architecture (Domain, Application, Infrastructure)
- MediatR for CQRS
- Entity Framework Core
- Domain-Driven Design patterns

**Layers:**
```
Ordering.API          - API endpoints and controllers
Ordering.Application  - Use cases, CQRS handlers
Ordering.Domain       - Domain models, aggregates, value objects
Ordering.Infrastructure - Data access, external services
```

**Domain Model:**
```
Order (Aggregate Root)
- OrderId
- CustomerId
- OrderDate
- Status
- Items (OrderItem list)
- TotalAmount

OrderItem (Entity)
- ProductId
- ProductName
- Quantity
- UnitPrice
- TotalPrice
```

## Design Patterns

### CQRS (Command Query Responsibility Segregation)
- Commands: Create, Update, Delete operations
- Queries: Read operations
- Implemented using MediatR

### Repository Pattern
- Abstraction over data access
- Used in services for data operations

### API Gateway Pattern (Planned)
- Single entry point for clients
- Request routing
- Authentication/Authorization
- Rate limiting

### Database per Service
- Each microservice has its own database
- Ensures loose coupling
- Allows independent scaling

### Event Sourcing
- Implemented in Catalog service using Marten
- Stores state changes as events
- Enables temporal queries and audit trails

## Infrastructure

### Containerization

All services are containerized using Docker:

```dockerfile
# Example Dockerfile structure
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Build steps...
FROM base AS final
# Runtime configuration...
```

### Docker Compose

Services are orchestrated using Docker Compose:
- Development: `docker-compose.override.yml`
- Production: `docker-compose.yml`

### Databases

**PostgreSQL Instances:**
- CatalogDB (Port 5432) - Catalog service
- BasketDB (Port 5433) - Basket service

**Redis:**
- Distributed cache (Port 6379) - Basket service caching

**SQLite:**
- Discount service - Embedded database

## API Documentation

All REST APIs are documented using Swagger/OpenAPI:
- Catalog API: http://localhost:5000/swagger
- Basket API: http://localhost:6001/swagger

## Security Considerations

### Current Implementation
- HTTPS support
- Environment-based configuration
- User secrets for sensitive data

### Planned Enhancements
- JWT authentication
- API Gateway authentication
- Rate limiting
- Input validation (FluentValidation)

## Scalability

### Horizontal Scaling
- Services can be scaled independently
- Load balancing through Docker/Kubernetes

### Caching Strategy
- Redis for distributed caching
- Reduces database load
- Improves response times

### Database Optimization
- Indexing on frequently queried fields
- Connection pooling
- Read replicas (planned)

## Monitoring & Observability (Planned)

### Logging
- Structured logging with Serilog
- Centralized log aggregation
- Log correlation across services

### Metrics
- Application Performance Monitoring (APM)
- Custom business metrics
- Health checks

### Tracing
- Distributed tracing
- Request correlation
- Performance bottleneck identification

## Future Enhancements

1. **API Gateway**
   - Ocelot or YARP
   - Centralized routing
   - Authentication/Authorization

2. **Service Discovery**
   - Consul or Eureka
   - Dynamic service registration

3. **Message Broker**
   - RabbitMQ or Azure Service Bus
   - Event-driven architecture
   - Asynchronous communication

4. **CI/CD Pipeline**
   - GitHub Actions
   - Automated testing
   - Container registry integration
   - Automated deployment

5. **Observability Stack**
   - ELK Stack (Elasticsearch, Logstash, Kibana)
   - Prometheus & Grafana
   - Jaeger for distributed tracing

6. **Resilience Patterns**
   - Circuit Breaker (Polly)
   - Retry policies
   - Timeout policies
   - Bulkhead isolation

## Development Workflow

1. **Local Development**
   - Run services individually with `dotnet run`
   - Use Docker Compose for integration testing

2. **Testing**
   - Unit tests for business logic
   - Integration tests for APIs
   - Contract tests for service communication

3. **Deployment**
   - Build Docker images
   - Push to container registry
   - Deploy using Docker Compose or Kubernetes

## References

- [Microservices Architecture](https://microservices.io/)
- [.NET Microservices](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/tags/domain%20driven%20design.html)
