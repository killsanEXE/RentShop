FROM node:alpine AS build
WORKDIR /app
COPY package.json ./
RUN npm install -g @angular/cli 
RUN npm install --save --legacy-peer-deps @ng-bootstrap/ng-bootstrap

COPY . .
RUN npm run build-docker

FROM nginx:alpine
COPY ./config/nginx.conf /etc/nginx/nginx.conf

COPY --from=build /app/dist/CLIENT /usr/share/nginx/html