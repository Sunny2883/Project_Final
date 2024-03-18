FROM node:20-alpine

RUN apk add --no-cache openssl

WORKDIR /project


RUN npm install -g @angular/cli


RUN openssl req -x509 -newkey rsa:2048 -keyout server.key -out server.crt -days 365 -nodes -subj "/CN=localhost"

COPY package*.json ./

RUN npm install

EXPOSE 4200


COPY . .

RUN npm install esbuild@0.20.1


CMD ["ng", "serve", "--ssl", "true", "--ssl-key", "server.key", "--ssl-cert", "server.crt", "--host", "0.0.0.0"]
