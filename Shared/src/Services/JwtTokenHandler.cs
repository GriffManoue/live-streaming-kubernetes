using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Shared.Services
{
    public class JwtTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _serviceJwtToken;

        public JwtTokenHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            // Get the service JWT from configuration or environment variable
            _serviceJwtToken = configuration["ServiceJwt:Token"] ?? string.Empty;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var token = httpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else if (!string.IsNullOrEmpty(_serviceJwtToken))
            {
                // Use the fallback service JWT for background/service-to-service calls
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _serviceJwtToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
