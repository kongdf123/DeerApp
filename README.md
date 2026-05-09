# Microservices Demo with .NET 8

A lightweight, production-style microservices demo built with:

* .NET 8
* ASP.NET Core Minimal APIs
* PostgreSQL
* Docker Compose
* YARP API Gateway
* RabbitMQ
* Polly Resilience Policies
* Serilog + Seq Observability

This project intentionally avoids heavy frameworks to help understand core microservice architecture concepts step-by-step.

---

# 🎯 Project Goals

This demo focuses on learning:

* microservice boundaries
* API Gateway pattern
* synchronous vs asynchronous communication
* distributed system resilience
* containerized development
* event-driven architecture
* centralized logging
* observability basics

The architecture is intentionally lightweight compared to frameworks like:

* ABP Framework
* eShopOnContainers

to make the core concepts easier to understand.

---

# 🏗 Architecture

```text id="0z26nq"
                    ┌──────────────┐
                    │  Gateway.API │
                    │     YARP     │
                    └──────┬───────┘
                           │
         ┌─────────────────┴─────────────────┐
         │                                   │
         ▼                                   ▼

┌──────────────────┐              ┌──────────────────┐
│   Catalog.API    │              │    Order.API     │
│                  │              │                  │
│ PostgreSQL       │              │ PostgreSQL       │
│ RabbitMQConsumer │              │ RabbitMQProducer │
└────────┬─────────┘              └────────┬─────────┘
         │                                  │
         └──────────────┬───────────────────┘
                        ▼

                 ┌────────────┐
                 │ RabbitMQ   │
                 └────────────┘

                        ▼

                 ┌────────────┐
                 │ Seq Logs   │
                 └────────────┘
```

---

# 🚀 Technologies

| Technology               | Purpose                |
| ------------------------ | ---------------------- |
| .NET 8                   | Backend runtime        |
| ASP.NET Core Minimal API | Lightweight APIs       |
| PostgreSQL               | Service databases      |
| Docker Compose           | Local orchestration    |
| YARP                     | API Gateway            |
| RabbitMQ                 | Async messaging        |
| Polly                    | Resilience policies    |
| Serilog                  | Structured logging     |
| Seq                      | Centralized log viewer |

---

# 📁 Solution Structure

```text id="jlwmz8"
src/
│
├── gateway/
│   └── Gateway.API
│
├── services/
│   ├── catalog/
│   │   └── Catalog.API
│   │
│   └── order/
│       └── Order.API
│
├── Shared.Contracts/
│
└── docker-compose.yml
```

---

# 🧠 Architecture Concepts Covered

---

## 1. Microservice Isolation

Each service owns:

* its own database
* its own API
* its own business logic

No shared database.

---

## 2. API Gateway Pattern

Implemented using:

* YARP

Benefits:

* centralized routing
* service abstraction
* future authentication layer
* rate limiting support

---

## 3. Resilience Patterns

Implemented using:

* Polly

Patterns:

* retries
* timeout policies
* circuit breakers

---

## 4. Event-Driven Communication

Implemented using:

* RabbitMQ

Benefits:

* loose coupling
* eventual consistency
* async workflows

---

## 5. Observability

Implemented using:

* Serilog
* Seq

Features:

* centralized logs
* correlation IDs
* structured events

---

# 🐳 Running the System

---

# Prerequisites

Install:

* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0?utm_source=chatgpt.com)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/?utm_source=chatgpt.com)

---

# Start Everything

From solution root:

```bash id="jlwm2m"
docker compose up --build
```

---

# Stop Containers

```bash id="jlwm9v"
docker compose down
```

---

# ⚠️ WARNING

Avoid:

```bash id="6jlwm1"
docker compose down -v
```

unless intentionally deleting PostgreSQL data volumes.

---

# 🌐 Service Endpoints

| Service             | URL                                              |
| ------------------- | ------------------------------------------------ |
| Gateway.API         | [http://localhost:7000](http://localhost:7000)   |
| Catalog.API         | [http://localhost:7001](http://localhost:7001)   |
| Order.API           | [http://localhost:7002](http://localhost:7002)   |
| RabbitMQ Management | [http://localhost:15672](http://localhost:15672) |
| Seq Logging UI      | [http://localhost:5341](http://localhost:5341)   |

---

# 🧪 Example API Calls

---

# Get Products

```http id="9jlwmc"
GET http://localhost:7000/catalog/products
```

---

# Create Order

```http id="5jlwmm"
POST http://localhost:7000/order/orders
Content-Type: application/json

{
  "productId": "11111111-1111-1111-1111-111111111111",
  "quantity": 2
}
```

---

# 🔄 Event Flow Example

```text id="6jlwm9"
Client
  ↓
Gateway.API
  ↓
Order.API
  ↓ publish event
RabbitMQ
  ↓ consume event
Catalog.API
```

---

# 📦 Docker Volumes

Persistent PostgreSQL storage is configured using Docker volumes:

```yaml id="5jlwmw"
volumes:
  postgres-catalog-data:
  postgres-order-data:
```

This prevents database loss when containers restart.

---

# 🧱 Database Management

This demo intentionally:

* DOES NOT auto-run EF Core migrations
* assumes manual database schema management

This better reflects many real production environments.

---

# 📚 Learning Roadmap

This project evolved through the following stages:

1. Minimal APIs
2. PostgreSQL + Docker
3. API Gateway (YARP)
4. Resilience (Polly)
5. Async Messaging (RabbitMQ)
6. Observability (Serilog + Seq)

---

# 🚀 Future Improvements

Potential next steps:

* JWT Authentication
* Keycloak integration
* OpenTelemetry tracing
* Prometheus metrics
* Grafana dashboards
* Redis caching
* Kubernetes deployment
* CQRS pattern
* Saga orchestration
* Outbox pattern

---

# 🧠 Key Learning Philosophy

This project intentionally prioritizes:

* clarity
* architecture understanding
* infrastructure fundamentals

over:

* enterprise abstraction
* heavy frameworks
* excessive boilerplate

The goal is to deeply understand how microservices work internally before adopting larger frameworks.

---

# 📄 License

MIT License.
