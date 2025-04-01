# Live Streaming Kubernetes Application

This project is a microservices-based live streaming platform designed to run on Kubernetes. It utilizes a repository pattern with a shared DbContext across backend services and features an Angular frontend.

## Architecture

The application consists of the following components:

### Backend Services

-   **Auth Service**: Handles user authentication and authorization using JWT tokens ✅
-   **User Service**: Manages user profiles and relationships ✅
-   **Stream Service**: Manages stream creation, updates, metadata, and integrates with NGINX RTMP ✅
-   **Analytics Service**: Tracks and provides analytics for streams and users (Planned)
-   **Recommendation Service**: Provides personalized stream and user recommendations (Planned)
-   **Database Management Service**: Handles database migrations and seeding ✅

### Infrastructure

-   **PostgreSQL**: Primary database for persistent storage.
-   **Redis**: Cache for frequently accessed data and session management.
-   **NGINX RTMP**: Handles incoming RTMP streams and converts them to HLS for playback.
-   **Kubernetes Ingress (NGINX)**: Routes external traffic to the appropriate services.

### Frontend

-   **Angular Client**: User interface for browsing streams, viewing content, managing settings, and user interactions. ✅

## Domain Models

-   **User**: Represents a user in the system with authentication and profile information.
-   **LiveStream**: Represents a live stream with metadata (name, description, category), stream key, URL, and viewer information.
-   **UserRelationship**: Represents follower/following relationships between users.

## Repository Pattern

The application uses the repository pattern with a shared `IDbContext` interface to provide a consistent data access layer across relevant backend services. This includes:

-   Generic `IRepository<T>` interface.
-   Implementation of the repository pattern in `Repository<T>`.
-   Service-specific DbContexts (e.g., `UserDbContext`, `StreamDbContext`) implementing `IDbContext`.

## Authentication

The application uses JWT (JSON Web Tokens) for authentication:

-   Token generation and validation handled by the Auth Service.
-   Password hashing using BCrypt.
-   Token-based authentication middleware used across protected service endpoints.
-   Secure API endpoints using the `[Authorize]` attribute.

## Getting Started

### Prerequisites

-   Docker and Docker Compose
-   .NET 9.0 SDK (or the version specified in `global.json`)
-   Node.js and npm (for Angular Client development)
-   Kubernetes cluster (e.g., Docker Desktop Kubernetes, Minikube, or a cloud provider)
-   `kubectl` command-line tool

### Development Setup

1.  **Clone the repository:**
    ```bash
    git clone <your-repository-url>
    cd live-streaming-kubernetes
    ```

2.  **Backend Setup (Docker Compose):**
    -   Ensure Docker Desktop (or your Docker environment) is running.
    -   Run the backend services, database, Redis, and Nginx RTMP:
        ```bash
        docker-compose up --build -d
        ```
        *(Use `--build` initially and when Dockerfiles change. `-d` runs in detached mode.)*
    -   The first time, migrations might need to be applied. You might need to run the DatabaseManagementService locally or exec into the container to apply migrations if not handled automatically on startup.

3.  **Frontend Setup (Angular CLI):**
    -   Navigate to the Client directory:
        ```bash
        cd Client
        ```
    -   Install dependencies:
        ```bash
        npm install
        ```
    -   Run the Angular development server:
        ```bash
        ng serve
        ```

4.  **Accessing the Application:**
    -   **Frontend:** `http://localhost:4200/`
    -   **Backend Services (Directly, if needed for testing/debugging):**
        -   User Service: `http://localhost:8001` (Swagger: `http://localhost:8001/swagger`)
        -   Auth Service: `http://localhost:8002` (Swagger: `http://localhost:8002/swagger`)
        -   Stream Service: `http://localhost:8003` (Swagger: `http://localhost:8003/swagger`)
        -   Database Management: `http://localhost:8006` (Swagger: `http://localhost:8006/swagger`)
    -   **NGINX RTMP:** `rtmp://localhost:1935/live` (Publishing endpoint)
    -   **HLS Playback (via Nginx):** `http://localhost:8080/hls/<stream_key>.m3u8`

### Kubernetes Deployment

1.  **Build and Push Docker Images:**
    -   Build images for each service and the client. Replace `yourdockerhubusername` with your actual Docker Hub username or registry path.
        ```bash
        # Backend Services
        docker build -t yourdockerhubusername/authservice:latest -f AuthService/Dockerfile .
        docker build -t yourdockerhubusername/userservice:latest -f UserService/Dockerfile .
        docker build -t yourdockerhubusername/streamservice:latest -f StreamService/Dockerfile .
        docker build -t yourdockerhubusername/databasemanagementservice:latest -f DatabaseManagementService/Dockerfile .

        # Infrastructure
        docker build -t yourdockerhubusername/nginx-rtmp-k8s:latest -f Nginx-RTMP/Dockerfile . # Assuming a custom Nginx build

        # Frontend
        docker build -t yourdockerhubusername/client-k8s:latest -f Client/Dockerfile . # Assuming a Dockerfile exists for the client

        # Push images
        docker push yourdockerhubusername/authservice:latest
        docker push yourdockerhubusername/userservice:latest
        docker push yourdockerhubusername/streamservice:latest
        docker push yourdockerhubusername/databasemanagementservice:latest
        docker push yourdockerhubusername/nginx-rtmp-k8s:latest
        docker push yourdockerhubusername/client-k8s:latest
        ```
    -   *(Note: You might need separate Dockerfiles optimized for Kubernetes deployment vs. local development)*

2.  **Update Kubernetes Manifests:**
    -   Ensure all `image:` references in the `.yaml` files within the `k8s` directories point to the images you pushed (e.g., `yourdockerhubusername/servicename:latest`).
    -   Configure secrets (Database connection strings, Redis, JWT secret) using Kubernetes Secrets. Apply them before deploying services. Example placeholders are in the `k8s` directories.
    -   Configure ConfigMaps if necessary (e.g., `auth-config.yaml`, `rtmp-config.yaml`).

3.  **Apply Kubernetes Manifests:**
    -   Apply infrastructure components first (Database, Redis):
        ```bash
        kubectl apply -f Database/PostgreSQL/ # Contains postgres-secrets.yaml, postgres-pvc.yaml, postgres-deployment.yaml, postgres-service.yaml
        kubectl apply -f Database/Redis/    # Contains redis-secrets.yaml, redis-deployment.yaml, redis-service.yaml
        ```
    -   Wait for databases to be ready.
    -   Apply the Database Management Service (to handle migrations):
        ```bash
        kubectl apply -f DatabaseManagementService/k8s/ # Contains db-management-deployment.yaml, db-management-service.yaml
        ```
    -   Wait for migrations to complete (check logs if necessary).
    -   Apply backend services, Nginx RTMP, and the client:
        ```bash
        kubectl apply -f AuthService/k8s/
        kubectl apply -f UserService/k8s/
        kubectl apply -f StreamService/k8s/
        kubectl apply -f Nginx-RTMP/k8s/
        kubectl apply -f Client/k8s/
        ```
    -   Apply the Ingress controller (if not already installed) and the application ingress rules:
        ```bash
        # Example using Nginx Ingress Controller
        kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.0/deploy/static/provider/cloud/deploy.yaml
        kubectl apply -f Client/k8s/client-ingress.yaml # Apply your specific ingress rules
        ```

4.  **Accessing via Kubernetes:**
    -   Find the external IP or DNS name assigned to your Ingress controller or LoadBalancer services (like `nginx-rtmp-service` if using LoadBalancer type).
    -   Access the frontend via the Ingress host (e.g., `http://<ingress-ip-or-dns>/`).
    -   Publish streams to `rtmp://<nginx-rtmp-external-ip>:1935/live`.

## Project Structure

```
live-streaming-kubernetes/
├── Client/                      # Angular frontend application
│   ├── src/                     # Source files
│   ├── k8s/                     # Kubernetes manifests for Client
│   ├── angular.json             # Angular configuration
│   └── package.json             # NPM dependencies
├── Shared/                      # Shared libraries and models (.NET)
│   ├── src/
│   │   ├── Data/                # Shared DbContext interface, Repository
│   │   ├── Interfaces/          # Shared service interfaces
│   │   ├── Models/              # Domain models and DTOs
│   │   └── Services/            # Shared service implementations (e.g., Cache)
│   └── DockerHub/               # Docker Hub related scripts (Optional)
├── AuthService/                 # Authentication microservice (.NET)
│   ├── src/
│   ├── k8s/                     # Kubernetes manifests
│   └── Dockerfile
├── UserService/                 # User management microservice (.NET)
│   ├── src/
│   ├── k8s/                     # Kubernetes manifests
│   └── Dockerfile
├── StreamService/               # Stream management microservice (.NET)
│   ├── src/
│   ├── k8s/                     # Kubernetes manifests
│   └── Dockerfile
├── DatabaseManagementService/   # Handles DB Migrations (.NET)
│   ├── src/
│   ├── k8s/                     # Kubernetes manifests
│   └── Dockerfile
├── Database/                    # Database and Cache setup
│   ├── PostgreSQL/              # PostgreSQL Kubernetes manifests
│   └── Redis/                   # Redis Kubernetes manifests
├── Nginx-RTMP/                  # Nginx RTMP configuration and Dockerfile
│   ├── nginx.conf               # Nginx configuration file
│   ├── k8s/                     # Kubernetes manifests
│   └── Dockerfile
├── compose.yaml                 # Docker Compose configuration for local dev
└── README.md                    # This file
```

## RTMP Integration

The platform integrates with NGINX RTMP for live video streaming:

-   **Stream Publishing**: Streamers publish their live feed to `rtmp://<server-ip-or-domain>:1935/live` using software like OBS, providing their unique stream key.
-   **Stream Keys**: Each user is assigned a unique, secure stream key generated via the Stream Service. This key authenticates the incoming stream.
-   **Event Handling**: The NGINX RTMP module is configured to notify the Stream Service via HTTP callbacks (`on_publish`, `on_publish_done`) when streams start and stop. The `RtmpEventController` in the Stream Service handles these callbacks to update stream/user status.
-   **Stream Playback**: NGINX converts the incoming RTMP stream into HLS (`.m3u8` playlists and `.ts` segments). Viewers watch the stream using an HLS-compatible player (like Shaka Player used in the Angular client) pointing to `http://<nginx-http-ip-or-domain>:8080/hls/<stream_key>.m3u8`.

## Contributing

Contributions are welcome! Please follow these general steps:

1.  Fork the repository.
2.  Create a feature branch: `git checkout -b feature/your-amazing-feature`.
3.  Make your changes and commit them: `git commit -m 'Add amazing feature'`.
4.  Push to the branch: `git push origin feature/your-amazing-feature`.
5.  Open a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details (if one exists).
