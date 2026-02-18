You are a senior .NET 8 enterprise software architect.

Generate a complete production-ready implementation for a dynamic Performance Criteria Management system for SmartBizOps.

This system allows defining performance criteria dynamically per role instead of hardcoding.

Tech stack:

Backend:
- .NET 8 Web API
- Entity Framework Core
- SQL Server
- Clean Architecture
- Repository Pattern
- Service Layer Pattern

Frontend:
- React OR Blazor (prefer Blazor Server if not specified)
- Dynamic form rendering based on API response
- Cards UI for criteria display
- Admin can create/edit criteria

AI Integration Ready:
- Criteria may be generated later via AI
- Database must support AI-generated and manual criteria
- Include CreatedByAI flag

System Requirements:

A role is created first.

Each role can have multiple performance criteria.

Each criteria has:

- Id
- RoleId
- Name
- Description
- Category
- WeightPercentage
- MeasurementMethod
- ScoringMethod
- TargetValue
- MinimumThreshold
- MaximumThreshold
- IsActive
- CreatedByAI
- CreatedDate
- ModifiedDate

Employees will be evaluated against criteria.

Include EmployeePerformance table.

Database Requirements:

Generate EF Core entities and SQL Server schema for:

Roles
PerformanceCriteria
Employees
EmployeePerformanceScores

Backend Requirements:

Generate:

DbContext

Entities

DTOs

Repositories:

IRoleRepository
IPerformanceCriteriaRepository

Services:

IRoleService
IPerformanceCriteriaService

Implementations

Controllers:

RoleController
PerformanceCriteriaController

Endpoints required:

GET /api/roles

POST /api/roles

GET /api/criteria/by-role/{roleId}

POST /api/criteria

PUT /api/criteria/{id}

DELETE /api/criteria/{id}

Frontend Requirements:

Generate frontend code that:

Fetches roles from API

Allows selecting role from dropdown

Fetches criteria based on selected role

Displays criteria dynamically in cards

Allows adding new criteria

Allows editing criteria

Architecture Requirements:

Use clean architecture with folders:

Domain
Application
Infrastructure
API

Include:

Entity models
DTOs
Interfaces
Service implementations
EF configurations
Controllers

Code must be production-ready, clean, and scalable.

Include comments explaining important parts.

Generate complete code step by step starting with:

1. Database entities
2. DbContext
3. Repository interfaces
4. Repository implementations
5. Services
6. Controllers
7. Frontend dynamic UI
