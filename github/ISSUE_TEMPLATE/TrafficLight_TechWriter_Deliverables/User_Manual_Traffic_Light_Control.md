# Traffic Light Control – User Manual

## 1. Introduction

The Traffic Light Control system is a small web service that simulates a single traffic light at an intersection. It automatically cycles between **Red → Green → Yellow → Red** and also supports:
- **Manual override** of the light color.
- **Pedestrian crossing** mode, which temporarily stops traffic.

This manual explains how a non‑developer (for example, a traffic operator or student tester) can **use** the system through a web browser or an HTTP client such as Postman.

---

## 2. Getting Access to the System

The system is exposed as a web API. Your project team or instructor will provide a base address such as:

- `http://localhost:5000` (running locally on your machine), or  
- `http://<server-name>:<port>` (if deployed on a server).

When running in development mode, the home page shows the **Swagger UI**, which is an interactive web page for trying out the API.

To open Swagger:

1. Start the application (your developer or QA teammate can do this).
2. Open a browser.
3. Go to the base address (for example `http://localhost:5000`).
4. You should see the **“Traffic Light Control API v1”** documentation.

You can use this page to try all actions without writing any code.

---

## 3. Main Concepts

- **Traffic light** – represents a single set of lights at one intersection.
- **Current state** – the color the light is currently showing:  
  - `Red` – traffic must stop.  
  - `Green` – traffic may go.  
  - `Yellow` – traffic should slow down and prepare to stop.
- **Automatic control** – the light changes state every few seconds without user input.
- **Manual override** – a user can temporarily force the light to a specific color.
- **Pedestrian crossing** – a mode where the system turns the light red for cars so pedestrians can cross safely.

---

## 4. Typical User Tasks

### 4.1 View the current light status

**Goal:** See which color the light is currently showing.

1. Open Swagger in your browser.
2. Find the **`TrafficLight`** controller.
3. Select the **`GET /api/TrafficLight/status`** endpoint.
4. Click **“Try it out”** → **“Execute”**.

You should see a JSON response similar to:

```json
{
  "currentState": "Green",
  "lastChanged": "2025-12-08T02:30:15.123Z"
}
```

- `currentState` – the current color.
- `lastChanged` – when the light last changed color.

You can repeat this call to watch the light change over time.

---

### 4.2 Manually set the light color

**Goal:** Force the light to Red, Yellow, or Green (for example, to clear an incident).

1. In Swagger, find **`POST /api/TrafficLight/manual`**.
2. Click **“Try it out”**.
3. In the `state` field (query parameter), type one of:
   - `Red`
   - `Yellow`
   - `Green`
4. Click **“Execute”**.

If successful, the API returns a confirmation message such as:

```text
Manual override: Red
```

> **Note:** Manual overrides still follow the internal safety logic. After some time the automatic timer may resume cycling the light.

---

### 4.3 Start a pedestrian crossing

**Goal:** Temporarily stop traffic so pedestrians can cross.

1. In Swagger, find **`POST /api/TrafficLight/pedestrian`**.
2. Click **“Try it out”**.
3. Click **“Execute”** (no input is required).

What happens:

- The light is changed to **Red** for vehicles.
- A short delay simulates pedestrians crossing.
- The light returns to **Green** and traffic resumes.

The API returns:

```text
Pedestrian crossing started
```

You can call **`GET /api/TrafficLight/status`** again to confirm the current state during and after the crossing.

---

## 5. Example Using Postman (Optional)

If you prefer Postman or another HTTP client instead of Swagger:

### 5.1 View status

- **Method:** `GET`  
- **URL:** `http://localhost:5000/api/TrafficLight/status`

### 5.2 Manual override

- **Method:** `POST`  
- **URL:** `http://localhost:5000/api/TrafficLight/manual?state=Red`

Replace `Red` with `Yellow` or `Green` if needed.

### 5.3 Pedestrian crossing

- **Method:** `POST`  
- **URL:** `http://localhost:5000/api/TrafficLight/pedestrian`

---

## 6. Troubleshooting

| Problem | Possible Cause | What to Do |
|--------|----------------|-----------|
| Browser shows “This site can’t be reached.” | The application is not running, or the port is wrong. | Ask your developer/QA teammate to confirm that the API is running and which URL to use. |
| Swagger page is empty or 404. | Not running in development or wrong base URL. | Double‑check the URL (for example, try `http://localhost:5000/swagger` if Swagger is not mapped to `/`). |
| API returns 500 (server error). | Internal exception in the service. | Try again. If it persists, report the time and action you took to the development team. |

---

## 7. Summary

As an end user, you only need three key actions:

1. **Check status** – `GET /api/TrafficLight/status`
2. **Override the light** – `POST /api/TrafficLight/manual?state=<Red|Yellow|Green>`
3. **Start a pedestrian crossing** – `POST /api/TrafficLight/pedestrian`

All of these can be performed through the **Swagger UI** without any programming knowledge.
