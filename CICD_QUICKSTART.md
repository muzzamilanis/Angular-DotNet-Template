# Quick Start: CI/CD Pipeline

## Overview
This project has automated CI/CD pipelines that run on every push and pull request.

## What Gets Tested

### On Every Push/PR:
- ✅ Backend builds (.NET 8.0)
- ✅ Frontend builds (Angular 19)
- ✅ Code linting (ESLint)
- ✅ TypeScript type checking
- ✅ Docker image builds
- ✅ Integration tests (Docker Compose)
- ✅ Security scanning (Trivy)

## Local Development

Before pushing, run these commands to catch issues early:

### Backend
```bash
cd Backend/AspNetCore/SimpleApi
dotnet restore
dotnet build --configuration Release
```

### Frontend
```bash
cd FrontEnd
npm install
npm run lint          # Check code quality
npm run build         # Build for production
npm test              # Run tests
```

### Mobile
```bash
cd Mobile/SmartBizApp
npm install
npm run lint          # Check code quality
```

## Workflow Triggers

### Automatic Triggers:
- Push to `main`, `master`, or `develop` branches
- Pull requests to `main`, `master`, or `develop` branches

### Manual Triggers:
1. Go to GitHub → Actions tab
2. Select a workflow
3. Click "Run workflow"

## Viewing Results

### Check Workflow Status:
1. Go to the **Actions** tab in GitHub
2. Click on the latest workflow run
3. View detailed logs for each job

### PR Status Checks:
- Status checks appear on your pull request
- All checks must pass before merging
- Click "Details" to see logs

## Common Issues

### Build Failures

**Backend:**
- Check for compilation errors in logs
- Verify all NuGet packages are available
- Ensure .NET 8.0 SDK compatibility

**Frontend:**
- Check for TypeScript errors
- Verify npm packages are compatible
- Check for ESLint issues

### Docker Build Failures
- Verify Dockerfile syntax
- Check if all required files exist
- Ensure environment variables are set

### Tests Failing
- Run tests locally first
- Check for environment-specific issues
- Review test logs in workflow details

## Status Badges

Add these to issues/PRs to show pipeline status:

- Backend: ![Backend CI](https://github.com/muzzamilanis/Angular-DotNet-Template/workflows/Backend%20CI%2FCD/badge.svg)
- Frontend: ![Frontend CI](https://github.com/muzzamilanis/Angular-DotNet-Template/workflows/Frontend%20CI%2FCD/badge.svg)
- Integration: ![Integration](https://github.com/muzzamilanis/Angular-DotNet-Template/workflows/Full%20Stack%20Integration/badge.svg)

## Getting Help

- Review detailed documentation in [CI_CD_SETUP.md](CI_CD_SETUP.md)
- Check workflow files in `.github/workflows/`
- Create an issue if pipelines consistently fail

## Tips for Success

1. **Run local builds** before pushing
2. **Check linting** to catch style issues
3. **Read error messages** in workflow logs
4. **Fix one issue at a time** for easier debugging
5. **Keep commits small** for faster pipeline runs

## Deployment

### Production Deployment:
- Triggered by creating a GitHub Release
- Builds Docker images with version tags
- Publishes artifacts for 30 days

### Manual Deployment:
1. Go to Actions → Deploy to Production
2. Click "Run workflow"
3. Select environment (staging/production)
4. Click "Run workflow"

## Environment Variables

Required secrets for deployment (configured in repository settings):
- `DOCKER_USERNAME` - Docker Hub username
- `DOCKER_PASSWORD` - Docker Hub password/token

## Pipeline Architecture

```
Push/PR → Main CI/CD Pipeline
            ↓
    ┌───────┼───────┬─────────┐
    ↓       ↓       ↓         ↓
Backend  Frontend Mobile  Integration
   CI       CI      CI      Tests
    ↓       ↓       ↓         ↓
    └───────┴───────┴─────────┘
              ↓
        Pipeline Summary
```

## Next Steps

1. Read [CI_CD_SETUP.md](CI_CD_SETUP.md) for detailed documentation
2. Review [.github/workflows/README.md](.github/workflows/README.md) for workflow details
3. Check [CONTRIBUTING.md](CONTRIBUTING.md) for contribution guidelines
