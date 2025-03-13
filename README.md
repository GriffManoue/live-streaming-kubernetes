# Live Streaming Kubernetes Application

This project is a microservices-based live streaming platform designed to run on Kubernetes. It follows a repository pattern with a shared DbContext across services.

## Architecture

The application consists of the following components:

### Backend Services

- **Auth Service**: Handles user authentication and authorization
- **User Service**: Manages user profiles and relationships
- **Stream Service**: Manages stream creation, updates, and metadata
- **Analytics Service**: Tracks and provides analytics for streams and users
- **Recommendation Service**: Provides personalized stream and user recommendations

### Infrastructure

- **PostgreSQL**: Primary database for persistent storage
- **Redis**: Cache for frequently accessed data and active streams
- **NGINX RTMP**: Handles incoming RTMP streams and HLS/DASH conversion
- **Ambassador API Gateway**: Routes requests to appropriate services

### Frontend

- **Angular Frontend**: User interface for the streaming platform

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
   - NGINX RTMP: rtmp://localhost:1935/live

### Kubernetes Deployment

1. Build and push the Docker images:
   ```
   docker build -t yourusername/user-service:latest -f UserService/Dockerfile .
   docker push yourusername/user-service:latest
   ```

2. Update the Kubernetes deployment files with your image registry:
   ```
   sed -i 's/${REGISTRY_URL}/yourusername/g' UserService/k8s/deployment.yaml
   ```

3. Apply the Kubernetes configurations:
   ```
   kubectl apply -f UserService/k8s/deployment.yaml
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
├── AuthService/                 # Authentication service (to be implemented)
├── StreamService/               # Stream management service (to be implemented)
├── AnalyticsService/            # Analytics service (to be implemented)
├── RecommendationService/       # Recommendation service (to be implemented)
└── compose.yaml                 # Docker Compose configuration
```

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Commit your changes: `git commit -am 'Add my feature'`
4. Push to the branch: `git push origin feature/my-feature`
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
