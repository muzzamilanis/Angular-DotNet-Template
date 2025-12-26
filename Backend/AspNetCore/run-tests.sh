#!/bin/bash
# Script to run all functional tests for the backend

cd "$(dirname "$0")"

echo "================================"
echo "Running Backend Functional Tests"
echo "================================"
echo ""

# Run tests
dotnet test SimpleApi.FunctionalTests/SimpleApi.FunctionalTests.csproj --verbosity normal

# Capture exit code
EXIT_CODE=$?

echo ""
echo "================================"
if [ $EXIT_CODE -eq 0 ]; then
    echo "✓ All tests passed successfully!"
else
    echo "✗ Some tests failed. Exit code: $EXIT_CODE"
fi
echo "================================"

exit $EXIT_CODE
