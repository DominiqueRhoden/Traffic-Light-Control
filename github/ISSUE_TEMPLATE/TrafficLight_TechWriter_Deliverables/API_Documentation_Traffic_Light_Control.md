# Traffic Light Control – API Documentation

This document describes the public HTTP API exposed by the Traffic Light Control Web API, as well as the main domain types that back it.

> Base URL examples  
> - Local development: `http://localhost:5000`  
> - Docker example: `http://localhost:5000` (mapped from container port 80)

All responses are JSON unless otherwise noted.

---

## 1. Domain Types

### 1.1 `TrafficLightState` (enum)

Defined in `TrafficLightControl.Models.TrafficLightState`.

Represents the possible colors for the simulated traffic light:

- `Red` – vehicles must stop.
- `Yellow` – vehicles must slow down and prepare to stop.
- `Green` – vehicles may go.

This enum is used both in the model and as a query parameter when manually overriding the light.

---

### 1.2 `TrafficLight` (class)

Defined in `TrafficLightControl.Models.TrafficLight`.

```csharp
public class TrafficLight
{
    public TrafficLightState CurrentState { get; private set; } = TrafficLightState.Red;
    public DateTime LastChanged { get; private set; } = DateTime.Now;

    public void ChangeState(TrafficLightState newState)
}
```

- **`CurrentState`**
  - Type: `TrafficLightState`
  - Description: The current color of the light.

- **`LastChanged`**
  - Type: `DateTime`
  - Description: Timestamp of the last state change.

- **`ChangeState(TrafficLightState newState)`**
  - Sets `CurrentState` to `newState`.
  - Updates `LastChanged` to `DateTime.Now`.
  - Writes a log message to the console (for debugging).

Serialized JSON example:

```json
{
  "currentState": "Green",
  "lastChanged": "2025-12-08T02:30:15.123Z"
}
```

---

### 1.3 `TrafficLightService` (class)

Defined in `TrafficLightControl.Services.TrafficLightService`.

Responsibility: encapsulates all business logic and timing for the light.

Key members:

```csharp
public class TrafficLightService
{
    public TrafficLightService();
    public TrafficLight GetStatus();
    public void SetManual(TrafficLightState newState);
    public Task PedestrianCrossAsync();
}
```

- **Constructor**
  - Starts a `System.Threading.Timer` that calls a private method every 5 seconds to advance the state:
    - Red → Green → Yellow → Red.
  - Uses a private lock object to synchronize access.

- **`TrafficLight GetStatus()`**
  - Returns the current `TrafficLight` instance.
  - Used by the `GET /api/TrafficLight/status` endpoint.

- **`void SetManual(TrafficLightState newState)`**
  - Immediately sets the light to `newState`.
  - Thread‑safe: uses a lock when mutating the underlying `TrafficLight`.

- **`Task PedestrianCrossAsync()`**
  - Sets the light to `Red`, waits for a fixed interval (e.g., 5 seconds), then sets it back to `Green`.
  - Logs messages like “Pedestrian crossing…” and “Traffic resumed” to the console.

---

## 2. HTTP API – `TrafficLightController`

All endpoints are provided by `TrafficLightControl.Controllers.TrafficLightController`.

### Base Route

```csharp
[ApiController]
[Route("api/[controller]")]
public class TrafficLightController : ControllerBase
```

Effective route prefix: **`/api/TrafficLight`**

---

### 2.1 GET `/api/TrafficLight/status`

Retrieve the current state of the traffic light.

**Request**

- Method: `GET`
- URL: `/api/TrafficLight/status`
- Body: *none*

**Response**

- Status: `200 OK`
- Body:

```json
{
  "currentState": "Green",
  "lastChanged": "2025-12-08T02:30:15.123Z"
}
```

**Notes**

- You can call this repeatedly to observe the automatic cycle.
- Safe to call at any time, even during pedestrian mode.

---

### 2.2 POST `/api/TrafficLight/manual`

Manually override the traffic light to a specific state.

**Request**

- Method: `POST`
- URL: `/api/TrafficLight/manual?state={state}`
- Query parameter:
  - `state` (required) – one of: `Red`, `Yellow`, `Green` (case‑insensitive model‑binding is handled by ASP.NET Core).

Example:

```http
POST /api/TrafficLight/manual?state=Red HTTP/1.1
Host: localhost:5000
```

**Response**

- Status: `200 OK`
- Body (plain text):

```text
Manual override: Red
```

**Notes**

- Manual override interacts with the same underlying `TrafficLight` as the automatic timer.
- After a manual override, future automatic transitions will continue to cycle through states from whatever you set.

---

### 2.3 POST `/api/TrafficLight/pedestrian`

Trigger a simulated pedestrian crossing.

**Request**

- Method: `POST`
- URL: `/api/TrafficLight/pedestrian`
- Body: *none*

**Behavior**

1. Immediately sets the light to `Red`.
2. Waits for a fixed duration (e.g., 5 seconds) to simulate pedestrians crossing.
3. Sets the light to `Green` and resumes normal operation.

**Response**

- Status: `200 OK`
- Body (plain text):

```text
Pedestrian crossing started
```

**Notes**

- While the pedestrian crossing is active, calls to `GET /api/TrafficLight/status` will show the temporary Red and then Green state.
- Intended to be called by UI controls (buttons, etc.) or automation scripts.

---

## 3. Swagger / OpenAPI

The API uses **Swashbuckle.AspNetCore** to generate an OpenAPI document and interactive Swagger UI.

In `Program.cs`:

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
...
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Traffic Light Control API V1");
        c.RoutePrefix = string.Empty;
    });
}
```

- In `Development`:
  - Swagger UI is served at the **root** (`/`).
  - OpenAPI JSON is available at `/swagger/v1/swagger.json`.

Developers can use this to:

- Explore endpoints.
- Generate API clients.
- Verify documentation and example requests.

---

## 4. Versioning

Semantic versioning is recommended for releases (e.g., `0.1.0`, `1.0.0`), in line with the project requirements. fileciteturn2file0  

API changes should follow these guidelines:

- **Patch (`x.y.Z`)** – bug fixes that do not change existing endpoints or response shapes.
- **Minor (`x.Y.z`)** – addition of new endpoints or optional fields.
- **Major (`X.y.z`)** – breaking changes, such as removing or renaming endpoints or changing response formats.

When making a breaking API change:

1. Update this API documentation.
2. Update the User Manual and Developer Guide.
3. Communicate the change in release notes for the new major version.

---

## 5. Summary

The Traffic Light Control API is intentionally small:

- One controller: `TrafficLightController`.
- Three endpoints: `status`, `manual`, and `pedestrian`.
- One main model: `TrafficLight` with `CurrentState` and `LastChanged`.

This simplicity makes it ideal for demonstrating clean documentation practices, CI/CD, and basic API design within a classroom project.
