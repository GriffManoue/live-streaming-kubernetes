#!/bin/bash
kubectl port-forward service/user-service 8081:80 &
kubectl port-forward service/auth-service 8082:80 &
kubectl port-forward service/stream-service 8083:80 &
kubectl port-forward service/db-management 8086:80 &
kubectl port-forward service/postgres 5432:5432 &

# Keep script running until Ctrl+C
echo "Port forwarding started. Press Ctrl+C to stop"
wait