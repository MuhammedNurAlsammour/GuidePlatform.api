# Multi-stage build for GuidePlatform API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["GuidePlatform.API/GuidePlatform.API.csproj", "GuidePlatform.API/"]
COPY ["GuidePlatform.Application/GuidePlatform.Application.csproj", "GuidePlatform.Application/"]
COPY ["GuidePlatform.Domain/GuidePlatform.Domain.csproj", "GuidePlatform.Domain/"]
COPY ["GuidePlatform.Infrastructure/GuidePlatform.Infrastructure.csproj", "GuidePlatform.Infrastructure/"]
COPY ["GuidePlatform.Persistence/GuidePlatform.Persistence.csproj", "GuidePlatform.Persistence/"]

# Restore dependencies
RUN dotnet restore "GuidePlatform.API/GuidePlatform.API.csproj"

# Copy source code
COPY . .
WORKDIR "/src/GuidePlatform.API"

# Build the application
RUN dotnet build "GuidePlatform.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "GuidePlatform.API.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app

# Install necessary packages for production
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000;https://+:5001
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "GuidePlatform.API.dll"]
