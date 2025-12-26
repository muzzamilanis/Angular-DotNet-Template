# =========================
# Build & Test stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files first (for caching)
COPY SimpleApi.csproj ./
COPY SimpleApi.FunctionalTests/SimpleApi.FunctionalTests.csproj ./SimpleApi.FunctionalTests/

# Restore dependencies
RUN dotnet restore SimpleApi.csproj
RUN dotnet restore SimpleApi.FunctionalTests/SimpleApi.FunctionalTests.csproj

# Copy the rest of the source
COPY . .

# Build API project
RUN dotnet build SimpleApi.csproj -c Release --no-restore

# Run tests (THIS is how tests are included correctly)
RUN dotnet test SimpleApi.FunctionalTests/SimpleApi.FunctionalTests.csproj \
    -c Release \
    --no-build \
    --logger trx

# Publish ONLY the API project
RUN dotnet publish SimpleApi.csproj \
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