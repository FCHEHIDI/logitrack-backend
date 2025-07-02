# Dockerfile Explanation for LogiTrack API

This document explains the production-ready Dockerfile used to containerize the LogiTrack ASP.NET Core API.

## What is Containerization?

Containerization is a technology that packages an application and all its dependencies into a single, portable unit called a container. Containers ensure that your app runs the same way in development, testing, and production, regardless of the underlying environment. They are lightweight, start quickly, and are isolated from other applications on the same host.

- **Portability:** Containers can run anywhere Docker (or compatible runtimes) are supported—on your laptop, in the cloud, or in CI/CD pipelines.
- **Isolation:** Each container has its own filesystem, processes, and network stack, separate from the host and other containers.
- **Consistency:** The environment inside the container is always the same, eliminating "works on my machine" problems.
- **Efficiency:** Containers share the host OS kernel, making them more lightweight than virtual machines.

> **In essence:**
> A Docker container acts like a lightweight, portable "virtual image" of your entire app—including all code, dependencies, and runtime settings. Any developer can run your app in their own environment without worrying about setup or compatibility. It's like downloading a project from GitHub and, instead of manually configuring everything, Docker instantly provides a ready-to-run environment that works the same everywhere.

## Dockerfile Overview

The Dockerfile is located at `LogiTrack/Dockerfile` and is designed for efficient, secure, and portable deployment of the API.

### Stages
1. **Base Image**
   - Uses `mcr.microsoft.com/dotnet/aspnet:9.0` as the runtime base.
   - Sets the working directory to `/app` and exposes port 5000.
2. **Build Image**
   - Uses `mcr.microsoft.com/dotnet/sdk:9.0` for building the application.
   - Copies the project file and restores dependencies.
   - Copies the rest of the source code and publishes the app to `/app/publish` in Release mode.
   - **Testing:** You can add a test step here (e.g., `RUN dotnet test`) to ensure the build passes all tests before publishing.
3. **Final Image**
   - Copies the published output from the build stage.
   - Sets the environment variable `ASPNETCORE_URLS` to `http://+:5000`.
   - Sets the entrypoint to run the API with `dotnet LogiTrack.dll`.

## Usage

To build and run the container locally:

```sh
# Build the Docker image
cd LogiTrack
# (from the project root)
docker build -t logitrack-api .

# Run the container
# (exposes port 5000)
docker run -p 5000:5000 logitrack-api
```

## Notes
- The image is multi-stage for smaller final size and better security.
- The published output is self-contained and ready for production.
- The container listens on port 5000 by default (see `ASPNETCORE_URLS`).
- You can use this image in any container orchestration platform (Kubernetes, Azure Container Apps, etc.).
- **Testing in CI/CD:** For best practices, add a `RUN dotnet test` step in the build stage of your Dockerfile or in your pipeline to ensure all tests pass before producing the final image.

---

For more details, see the official [ASP.NET Core Docker documentation](https://docs.microsoft.com/aspnet/core/host-and-deploy/docker/).
