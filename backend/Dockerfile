# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY Snippet.sln .
COPY Directory.Packages.props .

# Copy all project files (excluding tests - not needed for production)
COPY src/ src/
COPY libs/ libs/

# Restore dependencies (only for production projects)
RUN dotnet restore src/Host/Snippet.Web/Snippet.Web.csproj

# Build the application
RUN dotnet build src/Host/Snippet.Web/Snippet.Web.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish src/Host/Snippet.Web/Snippet.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Expose port 7000 (non-root port for better security)
EXPOSE 7000

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:7000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Snippet.Web.dll"]
