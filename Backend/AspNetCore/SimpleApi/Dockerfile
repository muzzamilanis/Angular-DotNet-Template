# =========================
# Build & Test stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files (from repo root perspective)
COPY Backend/AspNetCore/SimpleApi/SimpleApi/SimpleApi.csproj SimpleApi/
COPY Backend/AspNetCore/SimpleApi/SimpleApi.FunctionalTests/SimpleApi.FunctionalTests.csproj SimpleApi.FunctionalTests/

# Restore
RUN dotnet restore SimpleApi/SimpleApi.csproj
RUN dotnet restore SimpleApi.FunctionalTests/SimpleApi.FunctionalTests.csproj

# Copy everything from Backend/AspNetCore/SimpleApi
COPY Backend/AspNetCore/SimpleApi/ .

# Build API ONLY
RUN dotnet build SimpleApi/SimpleApi.csproj -c Release --no-restore

# Run tests (separate project)
RUN dotnet test SimpleApi.FunctionalTests/SimpleApi.FunctionalTests.csproj \
    -c Release \
    --no-restore

# Publish API
RUN dotnet publish SimpleApi/SimpleApi.csproj \
    -c Release \
    -o /app/publish \
    --no-build

# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output only
COPY --from=build /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENV ConnectionStrings__DefaultConnection=""
ENV Jwt__Key=""
ENV Jwt__Issuer="SimpleApi"
ENV Jwt__Audience="SimpleApi"
ENV Email__SmtpHost=""
ENV Email__SmtpPort="587"
ENV Email__Username=""
ENV Email__Password=""
ENV Email__FromEmail=""
ENV Email__FromName="SmartBizApp"

# Run as non-root user
RUN adduser --disabled-password --gecos '' appuser \
    && chown -R appuser /app
USER appuser

# Entry point
ENTRYPOINT ["dotnet", "SimpleApi.dll"]