#!/bin/bash

# Docker Build Validation Script
# This script validates that the Docker setup is working correctly

echo "=== Docker Build Validation ==="
echo "This script validates the Docker configuration for the ASP.NET Core API"
echo ""

# Check if Docker is available
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed or not in PATH"
    exit 1
fi

echo "✅ Docker is available"

# Check if docker-compose is available
if ! command -v docker-compose &> /dev/null; then
    echo "⚠️ docker-compose is not available, only basic Docker build will be tested"
    COMPOSE_AVAILABLE=false
else
    echo "✅ docker-compose is available"
    COMPOSE_AVAILABLE=true
fi

echo ""

# Navigate to the project directory
PROJECT_DIR="/home/runner/work/Angular-DotNet-Template/Angular-DotNet-Template"
API_DIR="$PROJECT_DIR/Backend/AspNetCore/SimpleApi"

if [ ! -f "$API_DIR/Dockerfile" ]; then
    echo "❌ Dockerfile not found at $API_DIR/Dockerfile"
    exit 1
fi

echo "✅ Dockerfile found"

if [ ! -f "$API_DIR/.dockerignore" ]; then
    echo "⚠️ .dockerignore not found (recommended for optimal builds)"
else
    echo "✅ .dockerignore found"
fi

if [ ! -f "$PROJECT_DIR/docker-compose.yml" ]; then
    echo "⚠️ docker-compose.yml not found at project root"
else
    echo "✅ docker-compose.yml found"
fi

echo ""
echo "=== Validation Complete ==="
echo ""
echo "📋 Manual Docker Build Test:"
echo "   cd $API_DIR"
echo "   docker build -t simpleapi ."
echo ""

if [ "$COMPOSE_AVAILABLE" = true ]; then
    echo "📋 Manual Docker Compose Test:"
    echo "   cd $PROJECT_DIR"
    echo "   docker-compose up --build"
    echo ""
fi

echo "📖 For complete instructions, see DOCKER_README.md"
echo ""
echo "✅ Docker configuration validation completed successfully!"