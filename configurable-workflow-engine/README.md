# Configurable Workflow Engine

A minimal .NET 8 Web API for defining and running state‑machine workflows in‑memory.
Includes Swagger/OpenAPI docs and a Dockerfile for local and containerized runs.

---

##  Setup

### 1. Clone the repo

```bash
git clone https://github.com/sk-883/shivam-Infonetica.git
cd configurable-workflow-engine
```

### 2. Or pull the published Docker image

```bash
docker pull vengeance883/configurable-workflow-engine:latest
```

---

##  Quick-start

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download) (for local runs)
* [Docker](https://www.docker.com/get-started) (optional, for container runs)
* (If using the published image) a Docker login:

  ```bash
  ```

docker login

````

---

### A. Run locally with .NET CLI

1. Restore & run:
   ```bash
   dotnet restore
   dotnet run
````

2. The API will listen on:

   ```
   http://localhost:5000
   ```

---

### B. Build & run in Docker

1. **Build** (from project root):

   ```bash
   docker build -t workflow-engine:local .
   ```
2. **Run** (maps `8080` in container → `5000` on host):

   ```bash
   docker run --rm -it \
     -e ASPNETCORE_URLS="http://+:8080" \
     -p 5000:8080 \
     workflow-engine:local
   ```
3. **Or** pull & run the remote image:

   ```bash
   docker pull your-dockerhub-username/configurable-workflow-engine:latest
   docker run --rm -it \
     -e ASPNETCORE_URLS="http://+:8080" \
     -p 5000:8080 \
     vengeance883/configurable-workflow-engine:latest
   ```
4. Confirm it’s up:

   ```bash
   curl http://localhost:5000/definitions
   # returns: []
   ```

---

##  API Documentation

* **Swagger UI (interactive)**
  Open in your browser:

  ```
  http://localhost:5000/
  ```

* **OpenAPI JSON**

  ```bash
  curl http://localhost:5000/swagger/v1/swagger.json -o openapi.json
  ```

* **OpenAPI YAML** (if you added a static `openapi.yaml` in `wwwroot/`):

  ```
  http://localhost:5000/openapi.yaml
  ```

---

##  Sample `curl` Commands

1. **List definitions**

   ```bash
   curl http://localhost:5000/definitions
   ```
2. **Create a workflow definition**

   ```bash
   curl -X POST http://localhost:5000/definitions \
     -H "Content-Type: application/json" \
     -d '{
       "id": "orderWorkflow",
       "name": "Order Processing",
       "states": [
         { "id": "created", "name": "Created", "isInitial": true,  "isFinal": false, "enabled": true },
         { "id": "paid",    "name": "Paid",    "isInitial": false, "isFinal": false, "enabled": true },
         { "id": "shipped", "name": "Shipped", "isInitial": false, "isFinal": true,  "enabled": true }
       ],
       "actions": [
         { "id": "pay",  "name": "Pay",  "enabled": true, "fromStates": ["created"], "toState": "paid" },
         { "id": "ship", "name": "Ship", "enabled": true, "fromStates": ["paid"],    "toState": "shipped" }
       ]
     }'
   ```
3. **Start a new instance**

   ```bash
   curl -X POST http://localhost:5000/definitions/orderWorkflow/instances
   ```
4. **Execute an action**

   ```bash
   curl -X POST http://localhost:5000/instances/<instanceId>/actions/pay
   ```
5. **Inspect the instance**

   ```bash
   curl http://localhost:5000/instances/<instanceId>
   ```

---

##  Environment Notes

* **Framework:** .NET 8 minimal API (Kestrel host)
* **Container port:** `8080` (mapped to host `5000`)
* **JSON Formatting:** indented for readability
* **Swagger/OpenAPI:** via Swashbuckle.AspNetCore

---

##  Assumptions & Limitations

* **In-memory storage only**
  All data is lost when the app stops.
* **No persistence layer**
  Swap in a database by implementing the repository interfaces.
* **Single-tenant, no authentication/authorization**
* **Minimal validation**

  * Exactly one initial state required
  * Unique state & action IDs
  * Actions must reference valid states
* **No update/delete operations**
  Definitions and instances are immutable once created.
* **No automated tests or logging**
  Logging, monitoring, and CI/CD are left as future enhancements.

---
