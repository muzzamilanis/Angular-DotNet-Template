# =========================
# Build & Test stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files (exact paths from repo root)
COPY Backend/AspNetCore/SimpleApi/SimpleApi.csproj ./SimpleApi/
COPY Backend/AspNetCore/SimpleApi/SimpleApi.FunctionalTests/SimpleApi.FunctionalTests.csproj ./SimpleApi.FunctionalTests/

# Restore
RUN dotnet restore ./SimpleApi/SimpleApi.csproj
RUN dotnet restore ./SimpleApi.FunctionalTests/SimpleApi.FunctionalTests.csproj

# Copy the rest of the source
COPY . .

# Build API only
RUN dotnet build Backend/AspNetCore/SimpleApi/SimpleApi.csproj -c Release --no-restore

# Run tests
RUN dotnet test Backend/AspNetCore/SimpleApi/SimpleApi.FunctionalTests/SimpleApi.FunctionalTests.csproj \
    -c Release \
    --no-build

# Publish API only
RUN dotnet publish Backend/AspNetCore/SimpleApi/SimpleApi.csproj \
    -c Release \
    -o /app/publish \
    --no-build
