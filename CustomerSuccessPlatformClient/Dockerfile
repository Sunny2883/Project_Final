FROM node:20-alpine

# Install OpenSSL
RUN apk add --no-cache openssl

# Set working directory
WORKDIR /project

# Install Angular CLI
RUN npm install -g @angular/cli

# Generate SSL certificates
RUN openssl req -x509 -newkey rsa:2048 -keyout server.key -out server.crt -days 365 -nodes -subj "/CN=localhost"

# Copy package files
COPY package*.json ./

# Install Node.js dependencies
RUN npm install

# Expose port
EXPOSE 4200

# Copy application files
COPY . .

# Install specific version of esbuild
RUN npm install esbuild@0.20.1

# Start Angular development server with SSL enabled
CMD ["ng", "serve", "--ssl", "true", "--ssl-key", "server.key", "--ssl-cert", "server.crt", "--host", "0.0.0.0"]
