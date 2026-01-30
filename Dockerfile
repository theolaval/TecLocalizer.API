# Multi-stage build for TEC Localizer

# Stage 1: Build Backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /src

# Copy solution and project files
COPY ["TecLocalizer.API/TecLocalizer.API.csproj", "TecLocalizer.API/"]
COPY ["TecLocalizer.BLL/TecLocalizer.BLL.csproj", "TecLocalizer.BLL/"]
COPY ["TecLocalizer.DAL/TecLocalizer.DAL.csproj", "TecLocalizer.DAL/"]
COPY ["TecLocalizer.DL/TecLocalizer.DL.csproj", "TecLocalizer.DL/"]
COPY ["TecLocalizer.API.sln", "."]

# Restore
RUN dotnet restore "TecLocalizer.API.sln"

# Copy source
COPY . .

# Build
WORKDIR "/src/TecLocalizer.API"
RUN dotnet build "TecLocalizer.API.csproj" -c Release -o /app/build

# Publish
FROM backend-build AS backend-publish
RUN dotnet publish "TecLocalizer.API.csproj" -c Release -o /app/publish

# Stage 2: Build Frontend
FROM node:18-alpine AS frontend-build
WORKDIR /app

COPY TecLocalizer.Web/package*.json ./
RUN npm ci

COPY TecLocalizer.Web/. .
RUN npm run build

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Copy backend publish
COPY --from=backend-publish /app/publish .

# Copy frontend dist
COPY --from=frontend-build /app/dist ./wwwroot

# Set environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000

EXPOSE 5000

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/api/vehicles/stats/summary || exit 1

ENTRYPOINT ["dotnet", "TecLocalizer.API.dll"]
