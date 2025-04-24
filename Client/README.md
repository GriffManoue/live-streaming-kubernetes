# KubeStream Client (Angular)

This is the frontend application for KubeStream, a live streaming platform running on Kubernetes. The client is built with Angular and provides a modern, responsive interface for users to watch streams, manage their profiles, and interact with the platform.

## Features
- User registration and login
- Browse and watch live streams (HLS playback via Shaka Player)
- Start and manage your own streams
- User profile management
- Follow/unfollow other users
- Real-time viewer counts and stream recommendations
- Responsive design

## Project Structure
- `src/` — Main Angular source code
  - `app/` — Components, services, routing, and modules
  - `assets/` — Static assets (images, styles)
  - `environments/` — Environment-specific configs
- `public/` — Public static files
- `k8s/` — Kubernetes manifests for deploying the client
- `angular.json` — Angular CLI configuration

## Getting Started

### Prerequisites
- [Node.js & npm](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli)

### Installation
1. Install dependencies:
   ```bash
   npm install
   ```
2. Start the development server:
   ```bash
   ng serve
   ```
   The app will be available at [http://localhost:4200](http://localhost:4200).

## Deployment
- Use the provided `k8s/` manifests to deploy the client as a containerized service in your Kubernetes cluster.
- The `Dockerfile` and `nginx.conf` are set up for serving the built Angular app with Nginx.

## Useful Commands
- `ng generate component <name>` — Generate a new component
- `ng generate service <name>` — Generate a new service
- `ng build` — Build the project

## Learn More
- [Angular Documentation](https://angular.io/docs)
- [Shaka Player Documentation](https://shaka-player-demo.appspot.com/docs/)

---

For backend and infrastructure setup, see the main project README.
