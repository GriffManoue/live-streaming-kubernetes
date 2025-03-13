# Implementation Plan for Remaining Services

This document outlines the implementation plan for the remaining microservices in the live streaming Kubernetes application.

## 1. Auth Service

### 1.1 Project Setup

1. Create a new ASP.NET Core Web API project:
   ```
   dotnet new webapi -n AuthService
   ```

2. Add reference to the Shared project:
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\Shared\Shared.csproj" />
   </ItemGroup>
   ```

3. Add required packages:
   ```xml
   <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.3" />
   <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.3.1" />
   ```

### 1.2 Implementation

1. Create the TokenService implementation:
   ```csharp
   public class TokenService : ITokenService
   {
       private readonly string _secretKey;
       private readonly string _issuer;
       private readonly string _audience;
       
       public TokenService(IConfiguration configuration)
       {
           _secretKey = configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException("Jwt:SecretKey");
           _issuer = configuration["Jwt:Issuer"] ?? "streaming-platform";
           _audience = configuration["Jwt:Audience"] ?? "streaming-users";
       }
       
       public string GenerateToken(User user)
       {
           // Implementation
       }
       
       public bool ValidateToken(string token)
       {
           // Implementation
       }
       
       public ClaimsPrincipal GetPrincipalFromToken(string token)
       {
           // Implementation
       }
   }
   ```

2. Create the AuthService implementation:
   ```csharp
   public class AuthService : IAuthService
   {
       private readonly IRepository<User> _userRepository;
       private readonly ITokenService _tokenService;
       private readonly IPasswordHasher _passwordHasher;
       
       public AuthService(IRepository<User> userRepository, ITokenService tokenService, IPasswordHasher passwordHasher)
       {
           _userRepository = userRepository;
           _tokenService = tokenService;
           _passwordHasher = passwordHasher;
       }
       
       public async Task<AuthResult> RegisterAsync(RegisterRequest request)
       {
           // Implementation
       }
       
       public async Task<AuthResult> LoginAsync(LoginRequest request)
       {
           // Implementation
       }
       
       public async Task<AuthResult> ValidateTokenAsync(string token)
       {
           // Implementation
       }
       
       public async Task RevokeTokenAsync(string token)
       {
           // Implementation
       }
   }
   ```

3. Create the AuthController:
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class AuthController : ControllerBase
   {
       private readonly IAuthService _authService;
       
       public AuthController(IAuthService authService)
       {
           _authService = authService;
       }
       
       [HttpPost("register")]
       public async Task<ActionResult<AuthResult>> Register([FromBody] RegisterRequest request)
       {
           // Implementation
       }
       
       [HttpPost("login")]
       public async Task<ActionResult<AuthResult>> Login([FromBody] LoginRequest request)
       {
           // Implementation
       }
       
       [HttpPost("validate")]
       public async Task<ActionResult<AuthResult>> ValidateToken([FromBody] ValidateTokenRequest request)
       {
           // Implementation
       }
       
       [HttpPost("revoke")]
       public async Task<ActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
       {
           // Implementation
       }
   }
   ```

### 1.3 Configuration

1. Update Program.cs to register services:
   ```csharp
   // Add shared services
   builder.Services.AddSharedServices(builder.Configuration);
   
   // Add auth services
   builder.Services.AddScoped<ITokenService, TokenService>();
   builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
   builder.Services.AddScoped<IAuthService, AuthService>();
   
   // Add JWT authentication
   builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options => {
           // Configure JWT options
       });
   ```

### 1.4 Deployment

1. Create Dockerfile
2. Create Kubernetes deployment files
3. Update docker-compose.yml

## 2. Stream Service

### 2.1 Project Setup

1. Create a new ASP.NET Core Web API project:
   ```
   dotnet new webapi -n StreamService
   ```

2. Add reference to the Shared project

3. Add required packages:
   ```xml
   <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.3" />
   ```

### 2.2 Implementation

1. Create the StreamService implementation:
   ```csharp
   public class StreamService : IStreamService
   {
       private readonly IRepository<LiveStream> _streamRepository;
       private readonly IRepository<User> _userRepository;
       private readonly IRepository<StreamMetadata> _metadataRepository;
       private readonly ICacheService _cacheService;
       
       public StreamService(
           IRepository<LiveStream> streamRepository,
           IRepository<User> userRepository,
           IRepository<StreamMetadata> metadataRepository,
           ICacheService cacheService)
       {
           _streamRepository = streamRepository;
           _userRepository = userRepository;
           _metadataRepository = metadataRepository;
           _cacheService = cacheService;
       }
       
       public async Task<StreamDto> GetStreamByIdAsync(Guid id)
       {
           // Implementation
       }
       
       public async Task<IEnumerable<StreamDto>> GetActiveStreamsAsync()
       {
           // Implementation
       }
       
       public async Task<IEnumerable<StreamDto>> GetStreamsByUserIdAsync(Guid userId)
       {
           // Implementation
       }
       
       public async Task<StreamDto> CreateStreamAsync(CreateStreamRequest request)
       {
           // Implementation
       }
       
       public async Task<StreamDto> UpdateStreamAsync(Guid id, UpdateStreamRequest request)
       {
           // Implementation
       }
       
       public async Task EndStreamAsync(Guid id)
       {
           // Implementation
       }
   }
   ```

2. Create the StreamController:
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   [Authorize]
   public class StreamController : ControllerBase
   {
       private readonly IStreamService _streamService;
       
       public StreamController(IStreamService streamService)
       {
           _streamService = streamService;
       }
       
       [HttpGet("{id:guid}")]
       [AllowAnonymous]
       public async Task<ActionResult<StreamDto>> GetStreamById(Guid id)
       {
           // Implementation
       }
       
       [HttpGet]
       [AllowAnonymous]
       public async Task<ActionResult<IEnumerable<StreamDto>>> GetActiveStreams()
       {
           // Implementation
       }
       
       [HttpGet("user/{userId:guid}")]
       [AllowAnonymous]
       public async Task<ActionResult<IEnumerable<StreamDto>>> GetStreamsByUserId(Guid userId)
       {
           // Implementation
       }
       
       [HttpPost]
       public async Task<ActionResult<StreamDto>> CreateStream([FromBody] CreateStreamRequest request)
       {
           // Implementation
       }
       
       [HttpPut("{id:guid}")]
       public async Task<ActionResult<StreamDto>> UpdateStream(Guid id, [FromBody] UpdateStreamRequest request)
       {
           // Implementation
       }
       
       [HttpPost("{id:guid}/end")]
       public async Task<ActionResult> EndStream(Guid id)
       {
           // Implementation
       }
   }
   ```

### 2.3 RTMP Integration

1. Create an RTMP event handler to process stream events:
   ```csharp
   [ApiController]
   [Route("api/rtmp")]
   public class RtmpEventController : ControllerBase
   {
       private readonly IStreamService _streamService;
       
       public RtmpEventController(IStreamService streamService)
       {
           _streamService = streamService;
       }
       
       [HttpPost("publish")]
       public async Task<ActionResult> OnPublish([FromBody] RtmpPublishEvent @event)
       {
           // Implementation to handle stream start
       }
       
       [HttpPost("publish_done")]
       public async Task<ActionResult> OnPublishDone([FromBody] RtmpPublishDoneEvent @event)
       {
           // Implementation to handle stream end
       }
   }
   ```

### 2.4 Configuration and Deployment

1. Update Program.cs to register services
2. Create Dockerfile
3. Create Kubernetes deployment files
4. Update docker-compose.yml

## 3. Analytics Service

### 3.1 Project Setup

1. Create a new ASP.NET Core Web API project:
   ```
   dotnet new webapi -n AnalyticsService
   ```

2. Add reference to the Shared project

3. Add required packages:
   ```xml
   <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.3" />
   ```

### 3.2 Implementation

1. Create the AnalyticsService implementation:
   ```csharp
   public class AnalyticsService : IAnalyticsService
   {
       private readonly IRepository<LiveStream> _streamRepository;
       private readonly IRepository<User> _userRepository;
       private readonly IRepository<StreamMetadata> _metadataRepository;
       private readonly ICacheService _cacheService;
       
       public AnalyticsService(
           IRepository<LiveStream> streamRepository,
           IRepository<User> userRepository,
           IRepository<StreamMetadata> metadataRepository,
           ICacheService cacheService)
       {
           _streamRepository = streamRepository;
           _userRepository = userRepository;
           _metadataRepository = metadataRepository;
           _cacheService = cacheService;
       }
       
       public async Task RecordStreamViewAsync(Guid streamId, Guid? userId)
       {
           // Implementation
       }
       
       public async Task<StreamAnalyticsDto> GetStreamAnalyticsAsync(Guid streamId)
       {
           // Implementation
       }
       
       public async Task<UserAnalyticsDto> GetUserAnalyticsAsync(Guid userId)
       {
           // Implementation
       }
   }
   ```

2. Create the AnalyticsController:
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class AnalyticsController : ControllerBase
   {
       private readonly IAnalyticsService _analyticsService;
       
       public AnalyticsController(IAnalyticsService analyticsService)
       {
           _analyticsService = analyticsService;
       }
       
       [HttpPost("view")]
       public async Task<ActionResult> RecordStreamView([FromBody] StreamViewRequest request)
       {
           // Implementation
       }
       
       [HttpGet("stream/{streamId:guid}")]
       [Authorize]
       public async Task<ActionResult<StreamAnalyticsDto>> GetStreamAnalytics(Guid streamId)
       {
           // Implementation
       }
       
       [HttpGet("user/{userId:guid}")]
       [Authorize]
       public async Task<ActionResult<UserAnalyticsDto>> GetUserAnalytics(Guid userId)
       {
           // Implementation
       }
   }
   ```

### 3.3 Configuration and Deployment

1. Update Program.cs to register services
2. Create Dockerfile
3. Create Kubernetes deployment files
4. Update docker-compose.yml

## 4. Recommendation Service

### 4.1 Project Setup

1. Create a new ASP.NET Core Web API project:
   ```
   dotnet new webapi -n RecommendationService
   ```

2. Add reference to the Shared project

3. Add required packages:
   ```xml
   <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.3" />
   ```

### 4.2 Implementation

1. Create the RecommendationService implementation:
   ```csharp
   public class RecommendationService : IRecommendationService
   {
       private readonly IRepository<LiveStream> _streamRepository;
       private readonly IRepository<User> _userRepository;
       private readonly IRepository<UserRelationship> _relationshipRepository;
       private readonly ICacheService _cacheService;
       
       public RecommendationService(
           IRepository<LiveStream> streamRepository,
           IRepository<User> userRepository,
           IRepository<UserRelationship> relationshipRepository,
           ICacheService cacheService)
       {
           _streamRepository = streamRepository;
           _userRepository = userRepository;
           _relationshipRepository = relationshipRepository;
           _cacheService = cacheService;
       }
       
       public async Task<IEnumerable<StreamDto>> GetRecommendedStreamsAsync(Guid userId)
       {
           // Implementation
       }
       
       public async Task<IEnumerable<UserDto>> GetRecommendedUsersToFollowAsync(Guid userId)
       {
           // Implementation
       }
   }
   ```

2. Create the RecommendationController:
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   [Authorize]
   public class RecommendationController : ControllerBase
   {
       private readonly IRecommendationService _recommendationService;
       
       public RecommendationController(IRecommendationService recommendationService)
       {
           _recommendationService = recommendationService;
       }
       
       [HttpGet("streams/{userId:guid}")]
       public async Task<ActionResult<IEnumerable<StreamDto>>> GetRecommendedStreams(Guid userId)
       {
           // Implementation
       }
       
       [HttpGet("users/{userId:guid}")]
       public async Task<ActionResult<IEnumerable<UserDto>>> GetRecommendedUsersToFollow(Guid userId)
       {
           // Implementation
       }
   }
   ```

### 4.3 Configuration and Deployment

1. Update Program.cs to register services
2. Create Dockerfile
3. Create Kubernetes deployment files
4. Update docker-compose.yml

## 5. API Gateway Configuration

### 5.1 Ambassador API Gateway

1. Create routing configuration for all services:
   ```yaml
   apiVersion: getambassador.io/v2
   kind: Mapping
   metadata:
     name: auth-service
   spec:
     prefix: /api/auth/
     service: auth-service:80
   ---
   apiVersion: getambassador.io/v2
   kind: Mapping
   metadata:
     name: user-service
   spec:
     prefix: /api/users/
     service: user-service:80
   ---
   apiVersion: getambassador.io/v2
   kind: Mapping
   metadata:
     name: stream-service
   spec:
     prefix: /api/streams/
     service: stream-service:80
   ---
   apiVersion: getambassador.io/v2
   kind: Mapping
   metadata:
     name: analytics-service
   spec:
     prefix: /api/analytics/
     service: analytics-service:80
   ---
   apiVersion: getambassador.io/v2
   kind: Mapping
   metadata:
     name: recommendation-service
   spec:
     prefix: /api/recommendations/
     service: recommendation-service:80
   ```

## 6. Frontend Implementation

1. Create Angular frontend project
2. Implement user authentication
3. Implement stream viewing and creation
4. Implement user profile and following
5. Implement analytics dashboard
6. Implement recommendations

## 7. Testing and Deployment

1. Write unit tests for all services
2. Write integration tests
3. Set up CI/CD pipeline
4. Deploy to Kubernetes cluster
5. Monitor and optimize performance
