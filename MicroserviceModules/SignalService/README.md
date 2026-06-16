# SignalService

SignalService is a modular .NET 10 microservice that provides:
- Web API endpoints
- gRPC service
- SignalR hubs
- Background job processing
- Azure Service Bus integration
- Functional test clients (Web API, gRPC, SignalR, Service Bus)

The project is fully containerized and includes Kubernetes deployment manifests.

---

## 📁 Project Structure
```
SignalService/
│
│   SignalService.slnx                     # Solution file
│
├── docker-compose/                        # Local container orchestration
│   │   docker-compose.dcproj
│   │   docker-compose.yml
│   │   launchSettings.json
│
├── k8s/                                   # Kubernetes deployment manifests
│   │   configmap.yaml
│   │   deployment.yaml
│   │   help.txt
│   │   ingress.yaml
│   │   namespace.yaml
│   │   secret.yaml
│   │   service.yaml
│
└── SignalService/                         # Main application source code
    │   .dockerignore
    │   appsettings.Development.json
    │   appsettings.json
    │   Dockerfile
    │   Program.cs
    │   SignalService.csproj
    │   SignalService.csproj.user
    │   _Dockerfile_
    │
    ├── Application/                       # Application layer (business logic)
    │   ├── Contracts/
    │   ├── Jobs/
    │   │   └── Middleware/
    │   ├── Queues/
    │   └── Services/
    │
    ├── bin/                               # Build output
    │
    ├── Controllers/                       # MVC & API controllers
    │
    ├── Domain/                            # Domain entities
    │
    ├── Hubs/                              # SignalR hubs
    │
    ├── Infrastructure/                    # EF Core, Auth, Health Checks
    │   ├── Auth/
    │   └── Health/
    │
    ├── Models/                            # View models
    │
    ├── obj/                               # Build artifacts
    │
    ├── Properties/                        # Launch settings
    │
    ├── Protos/                            # gRPC proto definitions
    │
    ├── Services/                          # gRPC service implementations
    │   └── Generated/
    │       └── Protos/                    # Auto-generated gRPC C# files
    │
    ├── Tests/                             # Functional test clients
    │   └── Functional/
    │
    ├── Views/                             # Razor views
    │   ├── FunctionalTests/
    │   ├── Home/
    │   └── Shared/
    │
    └── wwwroot/                           # Static web assets
        ├── css/
        ├── js/
        └── lib/                           # Bootstrap, jQuery, validation libs
```

---

## 🐳 Running with Docker

Build the image:

docker build -t signalservice:latest ./SignalService


Run with Docker Compose:

docker compose up --build

The service will be available at:

http://localhost:8080

---

## ☸️ Deploying to Kubernetes

All Kubernetes manifests are located in the `k8s/` folder.

Apply them in order:

kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/service.yaml
kubectl apply -f k8s/ingress.yaml


Check deployment status:

kubectl get pods -n signalservice
kubectl get svc -n signalservice
kubectl get ingress -n signalservice

If using Docker Desktop or Minikube, add to your hosts file:

127.0.0.1   signalservice.local

Then open:

http://signalservice.local

---

## 🧪 Functional Tests

Functional test clients are located in:

SignalService/Tests/Functional/

They use environment variables provided by:

- `configmap.yaml`
- `secret.yaml`

These clients test:
- Web API
- gRPC
- SignalR
- Azure Service Bus

---

## 📦 Technologies Used

⭐ Technologies Used
🟦 Backend / Runtime

    .NET 10 — primary runtime and framework

    ASP.NET Core MVC — controllers, routing, views

    C# 13 — language used across the project

🟩 Communication Protocols

    gRPC — high‑performance RPC communication

    gRPC-Web — browser-compatible gRPC

    Protocol Buffers — message serialization

    SignalR — real-time WebSocket communication

    REST Web API — standard HTTP endpoints

🟧 Cloud Messaging / Queueing

    Azure Service Bus — queue-based job processing

    Azure.Messaging.ServiceBus SDK — sending & receiving messages

🟨 Data & Persistence

    Entity Framework Core — ORM

    EF Core SQL Server Provider — SQL Server database support

    EF Core Design Tools — migrations, scaffolding

🟪 Frontend / UI

    Razor Views — server-rendered UI

    Bootstrap — styling

    jQuery — client-side scripting

    jQuery Validation + Unobtrusive Validation — form validation

🟥 Infrastructure & Deployment

    Docker — containerization

    Docker Compose — local multi-container orchestration

    Dockerfile — builds the application image

🟦 Kubernetes

    Kubernetes — container orchestration

    ConfigMap — non-secret configuration

    Secret — sensitive configuration

    Deployment — pod management

    Service (ClusterIP) — internal networking

    Ingress (NGINX) — external routing

    Namespace — resource isolation

🟫 Health & Diagnostics

    ASP.NET Core Health Checks

    Custom Health Checks (ServiceBusHealthCheck, WorkerHealthCheck)

    Logging Middleware

🟩 Testing

    Functional Test Clients:

        Web API Test Client

        gRPC Test Client

        SignalR Test Client

        Service Bus Test Client

These are located in:
SignalService/Tests/Functional/

🟦 Code Generation

    Grpc.Tools — generates C# classes from .proto files

    Generated Protos under:
    SignalService/Services/Generated/Protos/

🟫 Security

    API Key Authentication Handler

    User Secrets (development only)

🟩 Static Assets

    Bootstrap (CSS/JS)

    jQuery

    Static Web Assets under wwwroot/

---

## 📜 License

This project is licensed for internal development use.



