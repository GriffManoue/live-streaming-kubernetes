# Use the official PostgreSQL image
FROM postgres:latest

# Set environment variables
ENV POSTGRES_USER=postgres
ENV POSTGRES_PASSWORD=postgres
ENV POSTGRES_DB=streaming

# Expose PostgreSQL port
EXPOSE 5432

# The data directory is automatically created and initialized by the postgres image
# No need to manually create it in the Dockerfile

# Health check to ensure container is ready
HEALTHCHECK --interval=5s --timeout=5s --start-period=5s --retries=5 \
    CMD pg_isready -U postgres || exit 1