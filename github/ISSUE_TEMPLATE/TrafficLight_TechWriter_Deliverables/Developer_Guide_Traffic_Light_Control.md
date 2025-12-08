# Traffic Light Control – Developer Guide (Wiki Draft)

> This document is written for the **GitHub Wiki**. You can split it into multiple wiki pages if desired (for example: *Overview*, *Architecture*, *Running Locally*, *CI/CD*, *API*).

---

## 1. Project Overview

The Traffic Light Control project is a small ASP.NET Core Web API that simulates a traffic light intersection. It is used as a teaching example for:

- Clean separation between **models**, **services**, and **controllers**.
- Use of **Git** and **GitHub** (issues, branches, pull requests, releases). fileciteturn2file0  
- CI/CD with **GitHub Actions** and **Docker**.

Key features:

- Automatic state transitions between **Red**, **Yellow**, and **Green**.
- Manual override via API.
- Pedestrian crossing mode that safely moves the system to Red and then back to Green.
- Containerized deployment via Docker and GitHub Container Registry.

---

## 2. Repository Structure

```text
TrafficLightControl.sln        – Solution file
TrafficLightControl/           – ASP.NET Core Web API project
  Controllers/
    TrafficLightController.cs  – API endpoints
  Models/
    TrafficLight.cs            – Domain model (state + time of last change)
    TrafficLightState.cs       – Enum: Red, Yellow, Green
  Services/
    TrafficLightService.cs     – Business logic and timer-based state changes
  Program.cs                   – ASP.NET host configuration
.github/
  ISSUE_TEMPLATE/doc_task.yml  – Template for documentation work
  workflows/ci.yml             – CI/CD pipeline (build, test, image publish)
TrafficlightTests.cs           – Unit / integration tests
Docker-compose.yml             – Optional multi‑service composition
dockerfile.cs                  – Dockerfile for the application
```

---

## 3. Architecture

### 3.1 Models

- **`TrafficLight`**
  - Properties:
    - `TrafficLightState CurrentState` – current color.
    - `DateTime LastChanged` – last time the light changed.
  - Method:
    - `void ChangeState(TrafficLightState newState)` – sets the new state and updates `LastChanged`.

- **`TrafficLightState`**
  - Enum with values: `Red`, `Yellow`, `Green`.

### 3.2 Service Layer

- **`TrafficLightService`**
  - Holds a single `TrafficLight` instance.
  - Uses a `System.Threading.Timer` to automatically cycle states every *N* milliseconds.
  - Synchronizes access with a private lock object.
  - Public API:
    - `TrafficLight GetStatus()` – returns the current `TrafficLight`.
    - `void SetManual(TrafficLightState newState)` – allows manual overrides.
    - `Task PedestrianCrossAsync()` – switches to Red, waits, then returns to Green.

The service encapsulates all timing and state‑transition logic so that the controller stays thin.

### 3.3 Web API Layer

- **`TrafficLightController`**
  - Route prefix: `api/TrafficLight`.
  - Endpoints:
    - `GET /api/TrafficLight/status`
    - `POST /api/TrafficLight/manual`
    - `POST /api/TrafficLight/pedestrian`
  - Uses dependency injection to obtain a singleton `TrafficLightService`.

- **`Program.cs`**
  - Registers controller support, Swagger, and the `TrafficLightService`.
  - Configures Swagger UI as the root page in development.

---

## 4. Getting Started (Local Development)

### 4.1 Prerequisites

- .NET 8 SDK or compatible.
- Git.
- Docker (optional, for containerized run).
- A GitHub account with access to the repository.

### 4.2 Cloning the Repository

```bash
git clone <repo-url>
cd Traffic-Light-Control-main
```

Adjust the directory name if your clone path is different.

### 4.3 Running the API with `dotnet`

From the solution root:

```bash
dotnet restore
dotnet build
dotnet run --project TrafficLightControl/TrafficLightControl.csproj
```

By default the API listens on a port chosen by ASP.NET (commonly `http://localhost:5000` or `http://localhost:5075`). When running in `Development`, visiting the base URL in a browser will open the Swagger UI.

---

## 5. Running with Docker

> Exact image name and registry may be adjusted by your team.

### 5.1 Build Image Locally

```bash
docker build -t traffic-light-control:local .
```

### 5.2 Run Container

```bash
docker run -p 5000:80 traffic-light-control:local
```

Then open `http://localhost:5000` in your browser to access Swagger.

---

## 6. CI/CD Pipeline

A GitHub Actions workflow (`.github/workflows/ci.yml`) is configured to:

1. **Build and test** the application on:
   - Push to `main`.
   - Pull requests targeting `main`.
2. **Build and push a Docker image** to GitHub Container Registry when configured with:
   - `CR_PAT` – a personal access token with package permissions.
   - `GH_USERNAME` – your GitHub username.

Typical stages (simplified):

- `build-test` job
  - Check out repository.
  - Set up .NET SDK.
  - Run `dotnet restore`, `dotnet build`, and `dotnet test`.
- `publish-container` job
  - Build the Docker image.
  - Push to `ghcr.io/<username>/traffic-light-control:latest`.

---

## 7. Branching and Releases

The project is designed to follow a Git Flow‑style branching strategy. fileciteturn2file0  

Recommended branches:

- `main` – production‑ready code; tagged for releases (e.g., `v0.1.0`, `v1.0.0`).
- `develop` – integration branch where features are merged before release.
- `feature/*` – short‑lived branches for specific issues or features.

Release process:

1. Project manager creates issues describing features / tasks.
2. Developers and QA open feature branches from `develop`.
3. When work is complete and tests pass, a pull request is opened.
4. After review, the branch merges into `develop`.
5. For releases, `develop` merges into `main` and a semantic version tag is created:
   - `major.minor.patch` (e.g., `1.0.0`, `0.1.0`).

---

## 8. Documentation Responsibilities

As the **technical writer**, you maintain:

1. **User Guides** – how non‑technical users operate the system (see separate User Manual).
2. **Developer Guide / Wiki** – this document and related pages.
3. **API Documentation** – descriptions of endpoints and models.

Recommended workflow: for each new feature or change:

- Developer closes the coding task.
- QA confirms tests.
- Technical writer updates:
  - Wiki pages (overview, setup, feature behavior).
  - API section (new endpoints or parameters).
  - User manual, if user‑visible behavior changed.

---

## 9. Testing

Tests are located in `TrafficlightTests.cs` and cover core behavior such as:

- Correct state progression Red → Green → Yellow → Red.
- Correct behavior of manual overrides.
- Pedestrian crossing logic.

To run tests:

```bash
dotnet test
```

CI will also run these tests automatically on each push / PR.

---

## 10. Future Improvements

Potential enhancements that can be captured as GitHub issues:

- Support for multiple intersections instead of a single light.
- Configuration for timing (different cycle durations per light).
- Logging through ASP.NET Core logging instead of `Console.WriteLine`.
- Authentication / authorization for control endpoints.
