#!/bin/bash

# Database services
kubectl port-forward service/postgres 5432:5432 &
kubectl port-forward service/redis-service 6379:6379 &

# Core services
kubectl port-forward service/user-db-handler 8081:80 &
kubectl port-forward service/auth-service 8082:80 &
kubectl port-forward service/stream-db-handler 8083:80 &
kubectl port-forward service/stream-service 8084:80 &
kubectl port-forward service/viewer-service 8085:80 &
kubectl port-forward service/db-management 8086:80 &
kubectl port-forward service/follower-service 8087:80 &

# RTMP service (if local testing is needed)
kubectl port-forward service/nginx-rtmp-service 1935:1935 8080:8080 &

# Frontend service
kubectl port-forward service/client-service 4200:80 &

# Keep script running until Ctrl+C
echo "Port forwarding started. Press Ctrl+C to stop"
echo "Services are available at:"
echo "- User DB Handler: http://localhost:8081"
echo "- Auth Service: http://localhost:8082"
echo "- Stream DB Handler: http://localhost:8083"
echo "- Stream Service: http://localhost:8084"
echo "- Viewer Service: http://localhost:8085"
echo "- DB Management: http://localhost:8086"
echo "- Follower Service: http://localhost:8087"
echo "- Frontend: http://localhost:4200"
echo "- RTMP: rtmp://localhost:1935 and http://localhost:8080"
echo "- PostgreSQL: localhost:5432"
echo "- Redis: localhost:6379"
wait