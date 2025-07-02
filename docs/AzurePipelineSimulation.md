# Azure Pipeline Simulation for LogiTrack API

This document explains the simulated CI/CD pipeline for the LogiTrack ASP.NET Core API, as defined in `azure-pipelines.yml`.

## Pipeline Overview

The pipeline is designed for learning and demonstration purposes, simulating a real-world CI/CD process for a .NET 9 web API. It does not require actual Azure credits or deployment to Azure resources.

### Trigger
- The pipeline runs on every push to the `main` branch.

### Build Agent
- Uses the latest Windows VM image (`windows-latest`).

### Steps
1. **Install .NET 9 SDK**
   - Uses the `UseDotNet@2` task to install the .NET 9.0 SDK.
2. **Restore NuGet Packages**
   - Runs `dotnet restore` for the project.
3. **Build the Project**
   - Runs `dotnet build` in Release mode.
4. **Run Tests**
   - Runs `dotnet test` in Release mode. The pipeline continues even if tests fail (for learning/demo purposes).
5. **Publish Artifacts**
   - Runs `dotnet publish` to produce a self-contained build in the staging directory.
6. **Publish Build Artifacts**
   - Uploads the published output as a build artifact named `drop`.

## Notes
- This pipeline does not deploy to Azure App Service or any cloud resource, but the published artifact is ready for deployment.
- You can extend this pipeline to add deployment steps (e.g., to Azure Web App, Docker, etc.) if you have access.
- The pipeline is suitable for classroom, portfolio, or certification demonstration.

## File Reference
- **Pipeline YAML:** `azure-pipelines.yml`
- **Dockerfile:** `LogiTrack/Dockerfile` (for container builds, if desired)

---

For more details, see the official [Azure Pipelines documentation](https://docs.microsoft.com/azure/devops/pipelines/).
