kubectl apply -f Database/PostgreSQL/
kubectl apply -f Database/Redis/

sleep 10

kubectl apply -f DatabaseManagementService/k8s

sleep 5

kubectl apply -f UserService/k8s/
kubectl apply -f AuthService/k8s/
kubectl apply -f StreamService/k8s/
kubectl apply -f Nginx-RTMP/k8s
kubectl apply -f Client/k8s/

kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.0/deploy/static/provider/cloud/deploy.yaml
