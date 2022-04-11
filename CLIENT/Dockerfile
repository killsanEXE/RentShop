FROM node:alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
RUN npm install -g @angular/cli 
COPY . .
RUN npm run build-docker

FROM nginx:alpine
COPY ./config/nginx.conf /etc/nginx/nginx.conf

COPY --from=build /app/dist/CLIENT /usr/share/nginx/html