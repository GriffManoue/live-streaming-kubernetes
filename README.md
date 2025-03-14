# Live Streaming Kubernetes Application

This project is a microservices-based live streaming platform designed to run on Kubernetes. It follows a repository pattern with a shared DbContext across services.

## Architecture

The application consists of the following components:

### Backend Services

- **Auth Service**: Handles user authentication and authorization using JWT tokens ✅
- **User Service**: Manages user profiles and relationships ✅
- **Stream Service**: Manages stream creation, updates, and metadata, integrates with NGINX RTMP ✅
- **Analytics Service**: Tracks and provides analytics for streams and users ✅
- **Recommendation Service**: Provides personalized stream and user recommendations ✅

### Infrastructure

- **PostgreSQL**: Primary database for persistent storage
- **Redis**: Cache for frequently accessed data and active streams
- **NGINX RTMP**: Handles incoming RTMP streams and HLS/DASH conversion
- **Ambassador API Gateway**: Routes requests to appropriate services

### Frontend

- **Angular Frontend**: User interface for the streaming platform (to be implemented)

## Domain Models

- **User**: Represents a user in the system with authentication and profile information
- **LiveStream**: Represents a live stream with metadata and viewer information
- **StreamMetadata**: Contains additional information about a stream
- **UserRelationship**: Represents follower/following relationships between users

## Repository Pattern

The application uses the repository pattern with a shared DbContext to provide a consistent data access layer across all services. This includes:

- Generic `IRepository<T>` interface
- Implementation of the repository pattern in `Repository<T>`
- Shared `ApplicationDbContext` for database access

## Authentication

The application uses JWT (JSON Web Tokens) for authentication:

- Token generation and validation in the Auth Service
- Password hashing with BCrypt
- Token-based authentication across all services
- Secure API endpoints with [Authorize] attribute

## Getting Started

### Prerequisites

- Docker and Docker Compose
- .NET 9.0 SDK
- Kubernetes cluster (for production deployment)

### Development Setup

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/live-streaming-kubernetes.git
   cd live-streaming-kubernetes
   ```

2. Run the application using Docker Compose:
   ```
   docker-compose up
   ```

3. Access the services:
   - User Service: http://localhost:8001
   - Auth Service: http://localhost:8002
   - Stream Service: http://localhost:8003
   - Analytics Service: http://localhost:8004
   - Recommendation Service: http://localhost:8005
   - NGINX RTMP: rtmp://localhost:1935/live

### Kubernetes Deployment

1. Build and push the Docker images:
   ```
   docker build -t yourusername/user-service:latest -f UserService/Dockerfile .
   docker build -t yourusername/auth-service:latest -f AuthService/Dockerfile .
   docker build -t yourusername/stream-service:latest -f StreamService/Dockerfile .
   docker build -t yourusername/analytics-service:latest -f AnalyticsService/Dockerfile .
   docker build -t yourusername/recommendation-service:latest -f RecommendationService/Dockerfile .
   
   docker push yourusername/user-service:latest
   docker push yourusername/auth-service:latest
   docker push yourusername/stream-service:latest
   docker push yourusername/analytics-service:latest
   docker push yourusername/recommendation-service:latest
   ```

2. Update the Kubernetes deployment files with your image registry:
   ```
   sed -i 's/${REGISTRY_URL}/yourusername/g' */k8s/deployment.yaml
   ```

3. Apply the Kubernetes configurations:
   ```
   kubectl apply -f UserService/k8s/deployment.yaml
   kubectl apply -f AuthService/k8s/deployment.yaml
   kubectl apply -f StreamService/k8s/deployment.yaml
   kubectl apply -f AnalyticsService/k8s/deployment.yaml
   kubectl apply -f RecommendationService/k8s/deployment.yaml
   ```

## Project Structure

```
live-streaming-kubernetes/
├── Shared/                      # Shared libraries and models
│   ├── src/
│   │   ├── Data/                # Shared database context and repositories
│   │   ├── Extensions/          # Extension methods
│   │   ├── Interfaces/          # Shared interfaces
│   │   ├── Models/              # Domain models and DTOs
│   │   └── Services/            # Shared services
├── UserService/                 # User management service
│   ├── src/
│   │   ├── Controllers/         # API controllers
│   │   └── Services/            # Service implementations
│   ├── k8s/                     # Kubernetes deployment files
│   └── Dockerfile               # Docker build file
├── AuthService/                 # Authentication service
│   ├── src/
│   │   ├── Controllers/         # API controllers
│   │   └── Services/            # Service implementations
│   ├── k8s/                     # Kubernetes deployment files
│   └── Dockerfile               # Docker build file
├── StreamService/               # Stream management service
│   ├── src/
│   │   ├── Controllers/         # API controllers
│   │   └── Services/            # Service implementations
│   ├── k8s/                     # Kubernetes deployment files
│   └── Dockerfile               # Docker build file
├── AnalyticsService/            # Analytics service
│   ├── src/
│   │   ├── Controllers/         # API controllers
│   │   └── Services/            # Service implementations
│   ├── k8s/                     # Kubernetes deployment files
│   └── Dockerfile               # Docker build file
├── RecommendationService/       # Recommendation service
│   ├── src/
│   │   ├── Controllers/         # API controllers
│   │   └── Services/            # Service implementations
│   ├── k8s/                     # Kubernetes deployment files
│   └── Dockerfile               # Docker build file
└── compose.yaml                 # Docker Compose configuration
```

## RTMP Integration

The platform integrates with NGINX RTMP for live streaming:

- **Stream Publishing**: Streamers can publish to the RTMP server using OBS or similar software
- **Stream Keys**: Each user gets a unique stream key for authentication
- **Event Handling**: The Stream Service processes RTMP events (publish/publish_done)
- **Stream Playback**: Viewers can watch streams via HLS or DASH protocols

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Commit your changes: `git commit -am 'Add my feature'`
4. Push to the branch: `git push origin feature/my-feature`
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
