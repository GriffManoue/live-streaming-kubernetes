# Use an official Node.js runtime as a parent image
FROM node as builder

# Set the working directory
WORKDIR /app

# Copy package.json and package-lock.json
COPY /Client/package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the application code
COPY /Client/. .

# Build the Angular app
RUN npm run build

# Stage 2: Serve the app with Nginx
FROM nginx:alpine

# Copy the built Angular app from the previous stage
COPY --from=builder app/dist/client/browser /usr/share/nginx/html

# Copy the Nginx configuration file
COPY /Client/k8s/nginx.conf /etc/nginx/conf.d/default.conf

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]