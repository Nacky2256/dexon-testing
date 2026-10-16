# syntax=docker/dockerfile:1

# ---------- Build stage ----------
FROM node:24.21.0-alpine AS build
WORKDIR /app

# Install dependencies first (better layer caching)
COPY package.json package-lock.json ./
RUN npm ci

# Build the Angular app (production configuration)
COPY . ./
RUN npm run build -- --configuration production

# ---------- Runtime stage ----------
FROM nginx:1.27-alpine AS runtime

# SPA + API proxy configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Angular 22 emits the browser bundle under dist/<project>/browser
COPY --from=build /app/dist/piping-web/browser /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
