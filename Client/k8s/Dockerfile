# Use an official Node.js runtime as a parent image
FROM node:22

# Set the working directory
WORKDIR /usr/src/app

# Copy package.json and package-lock.json
COPY /Client/package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the application code
COPY /Client/. .

# Build the Angular app
RUN npm run build --prod

# Stage 2: Serve the app with Nginx
FROM nginx:alpine

# Copy the built Angular app from the previous stage
COPY --from=0 /usr/src/app/dist /usr/share/nginx/html

# Copy the Nginx configuration file
COPY /Client/k8s/nginx.conf /etc/nginx/nginx.conf

# Expose port 80
EXPOSE 80

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]