# FeatureGate.App

FeatureGate is a **runtime feature management service** built with **.NET 10** that enables applications to control feature behavior dynamically without redeploying code.

It supports **global feature states** and **target-based overrides** (User, Group, Region) and focuses on **correctness, validation, and maintainability.**

### 🎯Why FeatureGate?

In production systems, teams often need to:
  - Enable or disable features instantly
  - Roll out features gradually
  - Apply behavior only for certain users or regions
  - Avoid risky deployments for small changes
  - FeatureGate centralizes this logic into a single service so application code stays simple.

### ✨ Key Features

Global feature ON / OFF control
  - Feature overrides by:
     - User
     - Group
     - Region
  - Strict validation to prevent invalid or duplicate overrides
  - Deterministic override precedence
  - Redis-based caching for fast evaluation
  - Automatic cache invalidation on configuration changes

### 🏛️ Architecture Overview

The application follows **Clean Architecture** principles to keep business logic independent of frameworks and infrastructure.

    Client Request
         ↓
    API Controller
         ↓
    Application Service (business rules & validation)
         ↓
    Domain Entities (feature & override logic)
         ↓
    Infrastructure (Database / Cache)

## 🧱 Layer Responsibilities

| Layer | Responsibility |
|------|---------------|
| **Domain Layer** | Defines core business concepts such as `Feature`, `FeatureOverride`, and `OverrideType`. Contains no framework, database, or infrastructure dependencies. Represents **what the system is**. |
| **Application Layer** | Implements business rules and validations. Prevents duplicate overrides, enforces override precedence, and triggers cache invalidation. Represents **what the system does**. |
| **Infrastructure Layer** | Handles data persistence using Entity Framework Core. Integrates with PostgreSQL and Redis and provides repository implementations. Represents **how data is stored**. |
| **API Layer** | Exposes HTTP endpoints, validates requests, maps input/output models, and configures dependency injection and middleware. Represents **how the system is accessed**. |
| **Test Projects** | Contains unit tests for services and repositories. Covers validation paths and branch logic to ensure predictable and reliable behavior. |


## 🧩 Design Principles & Patterns Used

| Principle / Pattern | How It Is Used in This App |
|--------------------|---------------------------|
| **Single Responsibility** | Each class does one job (feature logic, database access, caching, or API handling). |
| **Open / Closed** | Base CRUD logic is reused and feature-specific rules are added without modifying it. |
| **Liskov Substitution** | Feature services extend base services without changing expected behavior. |
| **Interface Segregation** | Repositories and services expose only the methods they need. |
| **Dependency Inversion** | Application logic depends on interfaces, not database or cache code. |
| **Clean Architecture** | Domain, Application, Infrastructure, and API are separated into clear layers. |
| **Repository Pattern** | All database queries are handled through repository classes. |
| **Dependency Injection** | Services and repositories are provided through constructor injection. |
| **Template Method** | Common CRUD behavior is defined in a base service and extended where required. |
| **Strategy (Simple)** | Override logic changes based on target type (User, Group, Region). |
| **Cache-Aside** | Feature data is cached and cache is cleared when overrides change. |
| **DTO Pattern** | API input and output use DTOs instead of domain entities. |
| **Validation First** | Feature and override data is validated before being saved. |

## ⚙️ Technologies Used

| Area | Technology |
|-----|-----------|
| Backend | .NET 10 Web API |
| Database | PostgreSQL |
| ORM | Entity Framework Core |
| Cache | Redis |
| Containers | Docker & Docker Compose |
| Testing | xUnit, Moq |

---

## 🐳 Running the Application

This service depends on **PostgreSQL** and **Redis** and is designed to run using **Docker Compose**.

### 1️⃣ Clone the Repository

```bash
git clone https://github.com/soundar-14/FeatureGate.App.git
cd FeatureGate.App

### 2️⃣ Start Services Using Docker Compose

```bash
docker compose up --build

 Service               Purpose               Port
  --------------------- --------------------- --------
  **featureflag.api**   .NET 10 Web API       `8080`
  **postgres**          PostgreSQL database   `5432`
  **redis**             Redis cache           `6379`


## 🌐 API Access

 http://localhost:8080

## 🛑 Stopping the System

``` bash
docker compose down
```

Reset database volume:

``` bash
docker compose down -v

## 🧪 Running Tests

Tests do **not** require Docker.

``` bash
dotnet test
```

Testing stack:

-   **xUnit** -- Test framework
-   **Moq** -- Mocking dependencies
-   **FluentAssertions** -- Readable assertions

### Test Coverages

![Sonarquobe Result 1](https://raw.githubusercontent.com/soundar-14/FeatureGate.App/refs/heads/master/Coverage_snapshot1.png)
![Sonarquobe Result 1](https://raw.githubusercontent.com/soundar-14/FeatureGate.App/refs/heads/master/Coverage_snapshot2.png)
