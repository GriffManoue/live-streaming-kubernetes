# KubeStream: Live Streaming Platform on Kubernetes

KubeStream is a microservices-based live streaming platform designed to run on Kubernetes. It features real-time video streaming, user management, and scalable infrastructure using .NET, Angular, PostgreSQL, and Redis.

## Architecture Overview

- **Frontend:**
  - `Client/` — Angular application for user interaction, live stream viewing (HLS via Shaka Player), user profiles, and stream management.
- **Backend Microservices:**
  - `AuthService/` — Handles authentication and JWT token issuance.
  - `StreamService/` — Manages stream lifecycle (start, end, key generation, recommendations).
  - `StreamDbHandler/` — Handles stream data persistence (PostgreSQL) and caching (Redis).
  - `UserDbHandler/` — Manages user data and profiles.
  - `FollowerService/` — Manages user follow relationships.
  - `ViewerService/` — Handles viewer tracking and analytics.
- **Shared Library:**
  - `Shared/` — Common models, interfaces, and services (e.g., RedisCacheService, DTOs).
- **Infrastructure:**
  - `Database/` — PostgreSQL and Redis setup.
  - `Nginx-RTMP/` — RTMP server for ingesting live streams.
  - `k8s/` — Kubernetes manifests for all services.

## Key Features

- User registration, login, and JWT-based authentication
- Start, end, and manage live streams
- Stream key generation and validation
- Real-time viewer tracking and recommendations
- User profile management and following system
- Scalable, containerized microservices with Kubernetes
- Redis caching for performance
- PostgreSQL for persistent storage
- OpenAPI/Swagger documentation for APIs

## Development Setup

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/)
- [Node.js & npm](https://nodejs.org/) (for Angular client)
- [Angular CLI](https://angular.io/cli)
- Docker & Kubernetes (e.g., Minikube, Docker Desktop)
- PostgreSQL & Redis (local or via Docker)

### Running
 Run with Docker/Kubernetes:**
   - Use the manifests in each `k8s/` folder to deploy services.
   - Example (with kubectl):
     ```bash
     kubectl apply -f AuthService/k8s/
     kubectl apply -f StreamService/k8s/
     ...
     ```

## Project Structure

- `AuthService/` — Authentication microservice
- `StreamService/` — Stream management microservice
- `StreamDbHandler/` — Stream data persistence
- `UserDbHandler/` — User data persistence
- `FollowerService/` — Follower system
- `ViewerService/` — Viewer analytics
- `Client/` — Angular frontend
- `Shared/` — Shared .NET code (models, services)
- `Database/` — DB setup scripts
- `Nginx-RTMP/` — RTMP server config

## Additional Resources
- See each service's `README.md` or `k8s/` folder for deployment details.
- For Angular CLI usage, see [Angular CLI Reference](https://angular.dev/tools/cli).

---

For more details, see the documentation in each service and the `Client/README.md` for frontend-specific instructions.
