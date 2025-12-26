@echo off
REM Script to run all functional tests for the backend

cd /d "%~dp0"

echo ================================
echo Running Backend Functional Tests
echo ================================
echo.

REM Run tests
dotnet test SimpleApi.FunctionalTests\SimpleApi.FunctionalTests.csproj --verbosity normal

REM Capture exit code
set EXIT_CODE=%ERRORLEVEL%

echo.
echo ================================
if %EXIT_CODE% EQU 0 (
    echo All tests passed successfully!
) else (
    echo Some tests failed. Exit code: %EXIT_CODE%
)
echo ================================

exit /b %EXIT_CODE%
