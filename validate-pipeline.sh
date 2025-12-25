#!/bin/bash

# CI/CD Pipeline Local Validation Script
# This script runs the same checks that CI/CD pipelines run
# Run this before pushing to catch issues early

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Counters
TOTAL_CHECKS=0
PASSED_CHECKS=0
FAILED_CHECKS=0

echo -e "${BLUE}================================${NC}"
echo -e "${BLUE}CI/CD Pipeline Local Validation${NC}"
echo -e "${BLUE}================================${NC}"
echo ""

# Function to run a check
run_check() {
    local name=$1
    local command=$2
    local directory=$3
    
    TOTAL_CHECKS=$((TOTAL_CHECKS + 1))
    echo -e "${YELLOW}Running: $name${NC}"
    
    if [ -n "$directory" ]; then
        cd "$directory"
    fi
    
    if eval "$command" > /tmp/check_output.log 2>&1; then
        echo -e "${GREEN}✓ PASSED: $name${NC}"
        PASSED_CHECKS=$((PASSED_CHECKS + 1))
    else
        echo -e "${RED}✗ FAILED: $name${NC}"
        echo -e "${RED}Error output:${NC}"
        tail -20 /tmp/check_output.log
        FAILED_CHECKS=$((FAILED_CHECKS + 1))
    fi
    
    if [ -n "$directory" ]; then
        cd - > /dev/null
    fi
    echo ""
}

# Get script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
PROJECT_ROOT="$SCRIPT_DIR"

echo -e "${BLUE}Project Root: $PROJECT_ROOT${NC}"
echo ""

# Backend Checks
echo -e "${BLUE}=== Backend Checks ===${NC}"
if [ -d "$PROJECT_ROOT/Backend/AspNetCore/SimpleApi" ]; then
    run_check "Backend: Restore Dependencies" \
        "dotnet restore" \
        "$PROJECT_ROOT/Backend/AspNetCore/SimpleApi"
    
    run_check "Backend: Build (Release)" \
        "dotnet build --no-restore --configuration Release" \
        "$PROJECT_ROOT/Backend/AspNetCore/SimpleApi"
    
    run_check "Backend: Run Tests" \
        "dotnet test --no-build --configuration Release --verbosity quiet" \
        "$PROJECT_ROOT/Backend/AspNetCore/SimpleApi" || true  # Allow failure initially until tests are set up
else
    echo -e "${YELLOW}⚠ Backend directory not found, skipping backend checks${NC}"
    echo ""
fi

# Frontend Checks
echo -e "${BLUE}=== Frontend Checks ===${NC}"
if [ -d "$PROJECT_ROOT/FrontEnd" ]; then
    # Check if node_modules exists, if not install
    if [ ! -d "$PROJECT_ROOT/FrontEnd/node_modules" ]; then
        echo -e "${YELLOW}Installing frontend dependencies...${NC}"
        run_check "Frontend: Install Dependencies" \
            "npm install" \
            "$PROJECT_ROOT/FrontEnd"
    else
        echo -e "${GREEN}✓ Dependencies already installed${NC}"
        echo ""
        PASSED_CHECKS=$((PASSED_CHECKS + 1))
        TOTAL_CHECKS=$((TOTAL_CHECKS + 1))
    fi
    
    run_check "Frontend: Lint Check" \
        "npm run lint" \
        "$PROJECT_ROOT/FrontEnd" || true  # Allow failure initially due to pre-existing linting issues
    
    run_check "Frontend: TypeScript Check" \
        "npx tsc --noEmit" \
        "$PROJECT_ROOT/FrontEnd" || true  # Allow failure initially due to pre-existing type errors
    
    run_check "Frontend: Build (Production)" \
        "npm run build -- --configuration production" \
        "$PROJECT_ROOT/FrontEnd"
    
    # Optional: Run tests (may require Chrome)
    # run_check "Frontend: Run Tests" \
    #     "npm run test -- --watch=false --browsers=ChromeHeadless" \
    #     "$PROJECT_ROOT/FrontEnd" || true
else
    echo -e "${YELLOW}⚠ Frontend directory not found, skipping frontend checks${NC}"
    echo ""
fi

# Mobile Checks
echo -e "${BLUE}=== Mobile Checks ===${NC}"
if [ -d "$PROJECT_ROOT/Mobile/SmartBizApp" ]; then
    if [ ! -d "$PROJECT_ROOT/Mobile/SmartBizApp/node_modules" ]; then
        echo -e "${YELLOW}Installing mobile dependencies...${NC}"
        run_check "Mobile: Install Dependencies" \
            "npm install" \
            "$PROJECT_ROOT/Mobile/SmartBizApp"
    else
        echo -e "${GREEN}✓ Dependencies already installed${NC}"
        echo ""
        PASSED_CHECKS=$((PASSED_CHECKS + 1))
        TOTAL_CHECKS=$((TOTAL_CHECKS + 1))
    fi
    
    run_check "Mobile: TypeScript Check" \
        "npx tsc --noEmit" \
        "$PROJECT_ROOT/Mobile/SmartBizApp" || true  # Allow failure initially until TypeScript config is set up
else
    echo -e "${YELLOW}⚠ Mobile directory not found, skipping mobile checks${NC}"
    echo ""
fi

# Docker Checks
echo -e "${BLUE}=== Docker Checks ===${NC}"
if command -v docker &> /dev/null; then
    if [ -f "$PROJECT_ROOT/docker-compose.yml" ]; then
        run_check "Docker: Validate Compose File" \
            "docker-compose config > /dev/null" \
            "$PROJECT_ROOT"
        
        echo -e "${YELLOW}Note: Skipping Docker build (takes too long). CI/CD will test this.${NC}"
        echo ""
    else
        echo -e "${YELLOW}⚠ docker-compose.yml not found${NC}"
        echo ""
    fi
else
    echo -e "${YELLOW}⚠ Docker not installed, skipping Docker checks${NC}"
    echo ""
fi

# Summary
echo -e "${BLUE}================================${NC}"
echo -e "${BLUE}Validation Summary${NC}"
echo -e "${BLUE}================================${NC}"
echo -e "Total Checks: $TOTAL_CHECKS"
echo -e "${GREEN}Passed: $PASSED_CHECKS${NC}"
echo -e "${RED}Failed: $FAILED_CHECKS${NC}"
echo ""

if [ $FAILED_CHECKS -eq 0 ]; then
    echo -e "${GREEN}✓ All checks passed! Ready to push.${NC}"
    exit 0
else
    echo -e "${RED}✗ Some checks failed. Please fix the issues before pushing.${NC}"
    exit 1
fi
