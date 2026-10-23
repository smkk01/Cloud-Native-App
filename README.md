# SmartNote Cloud-Native Web API

SmartNote is a cloud-native ASP.NET Core Web API built with .NET 10. It can be
run locally with .NET Aspire or Docker Compose, containerized with Docker, and
automatically deployed to Azure Container Apps through GitHub Actions.

## Architecture

| Component | Description |
| --- | --- |
| `SmartNote.Api` | ASP.NET Core minimal Web API. |
| `SmartNote.AppHost` | .NET Aspire AppHost that orchestrates the API, PostgreSQL, and Redis locally. |
| `SmartNote-ServiceDefaults` | Shared Aspire service-discovery, resiliency, and telemetry configuration. |
| `docker-compose.yml` | Docker Compose configuration for the API and PostgreSQL. |
| `.github/workflows/ci.yml` | CI/CD workflow that builds, tests, publishes, and deploys the API. |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) for Azure deployment

## Run locally

### With .NET Aspire

The AppHost starts SmartNote API, PostgreSQL, and Redis:

```powershell
dotnet run --project .\SmartNote.AppHost\SmartNote.AppHost.csproj
```

Use the Aspire dashboard URL printed in the terminal to see running resources,
endpoints, logs, and telemetry.

### Run only the API

```powershell
dotnet restore .\SmartNote.slnx
dotnet run --project .\SmartNote.Api\SmartNote.Api.csproj
```

The API currently exposes:

```http
GET /weatherforecast
```

See [SmartNote.Api.http](./SmartNote.Api/SmartNote.Api.http) for an example
request.

### With Docker Compose

Set a secure local PostgreSQL password, then start the containers:

```powershell
$env:POSTGRES_PASSWORD = "use-a-strong-local-password"
docker compose up --build
```

The API is available at `http://localhost:8080`; PostgreSQL is exposed on
`localhost:5432`.

Stop the environment:

```powershell
docker compose down
```

## Build and test

```powershell
dotnet restore .\SmartNote.slnx
dotnet test .\SmartNote.slnx --no-restore
dotnet build .\SmartNote.slnx --no-restore
```

## Build and run the container

Build from the repository root. The Dockerfile requires the repository root as
its build context.

```powershell
docker build -f .\SmartNote.Api\Dockerfile -t smartnote-api:local .
docker run --rm -p 8080:8080 smartnote-api:local
```

## CI/CD and Azure deployment

The GitHub Actions workflow in [.github/workflows/ci.yml](./.github/workflows/ci.yml)
runs for pushes and pull requests targeting `main`.

For a push to `main`, it:

1. Restores dependencies, runs tests, and builds the solution.
2. Builds the Docker image from `SmartNote.Api/Dockerfile`.
3. Pushes `smartnote-api:<commit-sha>` to Docker Hub.
4. Signs in to Azure and updates the `smartnote-cloud-api` Container App in
   the `SmartNoteResourceGroup` resource group.

Configure these repository secrets in GitHub before pushing to `main`:

| Secret | Purpose |
| --- | --- |
| `DOCKER_USERNAME` | Docker Hub username or namespace. |
| `DOCKER_PASSWORD` | Docker Hub access token with image push permission. |
| `AZURE_CREDENTIALS` | Valid JSON credentials for an Azure service principal that can update the target Container App. |

Create service-principal credentials scoped to the resource group:

```powershell
az ad sp create-for-rbac `
  --name "github-smartnote-deploy" `
  --role contributor `
  --scopes "/subscriptions/<SUBSCRIPTION_ID>/resourceGroups/SmartNoteResourceGroup" `
  --sdk-auth
```

Save the entire JSON output, including the opening and closing braces, as the
`AZURE_CREDENTIALS` GitHub secret. Do not add it to source control.

## Project structure

```text
.
|-- .github/workflows/ci.yml
|-- SmartNote.Api/
|   |-- Dockerfile
|   |-- Program.cs
|   `-- SmartNote.Api.csproj
|-- SmartNote.AppHost/
|-- SmartNote-ServiceDefaults/
|-- SmartNote.slnx
`-- docker-compose.yml
```
