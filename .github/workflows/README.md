# CI/CD Pipeline Documentation

This directory contains GitHub Actions workflows for automated continuous integration and deployment of SmartBizApp.

## Workflows Overview

### 1. Main CI/CD Pipeline (`main-ci.yml`)
**Trigger:** Push/PR to main, master, or develop branches

The main orchestrator workflow that:
- Detects which components have changed
- Triggers appropriate component-specific workflows
- Provides a comprehensive pipeline summary

**Features:**
- Smart change detection to run only necessary workflows
- Parallel execution of independent jobs
- Consolidated status reporting

### 2. Backend CI (`backend-ci.yml`)
**Trigger:** Changes to `Backend/**` or push/PR to main branches

Validates the ASP.NET Core backend:
- ✅ Restore NuGet dependencies
- ✅ Build with .NET 8.0
- ✅ Run unit tests (if available)
- ✅ Publish release artifacts
- ✅ Validate Docker image build

**Artifacts:** Backend build files retained for 7 days

### 3. Frontend CI (`frontend-ci.yml`)
**Trigger:** Changes to `FrontEnd/**` or push/PR to main branches

Validates the Angular frontend:
- ✅ Install npm dependencies
- ✅ Run ESLint linting
- ✅ Execute unit tests with Karma/Jasmine
- ✅ Build production-optimized bundle
- ✅ Validate Docker image build

**Artifacts:** Frontend dist files retained for 7 days

### 4. Mobile CI (`mobile-ci.yml`)
**Trigger:** Changes to `Mobile/**` or push/PR to main branches

Validates the React Native mobile app:
- ✅ Install npm dependencies
- ✅ Run linting
- ✅ TypeScript type checking
- ✅ Execute tests (if available)

### 5. Integration Tests (`integration.yml`)
**Trigger:** Push/PR to main branches or changes to Docker files

Full-stack integration testing:
- ✅ Docker Compose orchestration
- ✅ Multi-container deployment validation
- ✅ API health checks
- ✅ Database connectivity verification
- ✅ Service startup verification

**Features:**
- Automatic environment configuration
- Health check with retry logic
- Comprehensive logging on failure

### 6. Code Quality (`code-quality.yml`)
**Trigger:** Push/PR to main branches

Enforces code quality standards:
- ✅ ESLint validation for frontend
- ✅ Prettier formatting checks
- ✅ TypeScript type checking
- ✅ .NET code analysis
- ✅ Security vulnerability scanning (Trivy)
- ✅ Dependency review for PRs

### 7. Deployment (`deploy.yml`)
**Trigger:** Release publication or manual workflow dispatch

Production deployment workflow:
- ✅ Build optimized backend artifacts
- ✅ Build optimized frontend artifacts
- ✅ Build and push Docker images
- ✅ Version tagging
- ✅ Artifact retention for 30 days

**Docker Images:**
- `smartbizapp-backend:latest` and `smartbizapp-backend:<version>`
- `smartbizapp-frontend:latest` and `smartbizapp-frontend:<version>`

## Workflow Execution Flow

```
┌─────────────────┐
│   Push/PR       │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Main CI/CD     │
│  (Detect Changes)│
└────────┬────────┘
         │
    ┌────┴────┬────────┬────────┐
    ▼         ▼        ▼        ▼
┌────────┐ ┌───────┐ ┌──────┐ ┌────────────┐
│Backend │ │Frontend│ │Mobile│ │Integration │
│  CI    │ │  CI   │ │  CI  │ │   Tests    │
└────────┘ └───────┘ └──────┘ └────────────┘
    │         │        │         │
    └─────────┴────────┴─────────┘
              │
              ▼
    ┌──────────────────┐
    │ Pipeline Summary │
    └──────────────────┘
```

## Required Secrets

For full functionality, configure these secrets in your repository settings:

### Docker Hub (for deployment)
- `DOCKER_USERNAME`: Docker Hub username
- `DOCKER_PASSWORD`: Docker Hub password or access token

### Optional Secrets
- Add environment-specific secrets for deployment workflows

## Badge URLs

Add these badges to your README.md:

```markdown
![Backend CI](https://github.com/muzzamilanis/Angular-DotNet-Template/workflows/Backend%20CI%2FCD/badge.svg)
![Frontend CI](https://github.com/muzzamilanis/Angular-DotNet-Template/workflows/Frontend%20CI%2FCD/badge.svg)
![Integration](https://github.com/muzzamilanis/Angular-DotNet-Template/workflows/Full%20Stack%20Integration/badge.svg)
![Code Quality](https://github.com/muzzamilanis/Angular-DotNet-Template/workflows/Code%20Quality/badge.svg)
```

## Manual Workflow Triggers

All workflows can be manually triggered via GitHub Actions UI:
1. Go to "Actions" tab
2. Select the workflow
3. Click "Run workflow"
4. Choose the branch

## Customization

### Adjusting Test Execution
Tests are set to `continue-on-error: true` to prevent pipeline failures during initial setup. Once tests are stable:

1. Remove `continue-on-error: true` from test steps
2. Ensure all tests pass before merging

### Modifying Triggers
Edit the `on:` section in each workflow to customize:
- Branch patterns
- Path filters
- Event types

### Cache Configuration
Workflows use GitHub Actions cache for:
- npm dependencies (Node.js)
- Docker layer caching
- NuGet packages (.NET)

## Troubleshooting

### Workflow Not Triggering
- Check path filters match your changes
- Verify branch name matches trigger patterns
- Check workflow file syntax (YAML validation)

### Docker Build Failures
- Ensure Dockerfile exists in specified paths
- Check Docker Compose configuration
- Review environment variable requirements

### Test Failures
- Review test logs in workflow run details
- Check for missing dependencies
- Verify test configuration files

### Integration Test Issues
- Check Docker Compose service health
- Review service logs (captured automatically)
- Verify environment variables are set correctly

## Best Practices

1. **Keep workflows focused**: Each workflow handles a specific concern
2. **Use caching**: Speed up builds with dependency caching
3. **Fail fast**: Run quick checks (linting) before expensive operations (builds)
4. **Artifact retention**: Balance storage costs with retention periods
5. **Security scanning**: Regularly update security scanning tools

## Monitoring

Monitor workflow status:
- GitHub Actions UI: Real-time execution
- Email notifications: Configure in GitHub settings
- Status checks: Required for PR merges
- Pipeline summaries: View in workflow run details

## Support

For issues or questions about CI/CD:
1. Check workflow run logs
2. Review this documentation
3. Create an issue in the repository
