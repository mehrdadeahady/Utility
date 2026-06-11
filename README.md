# **Utility — Unified Dashboard for Multi‑Protocol Microservices**

*Utility* is a unified, extensible dashboard built with *Next.js* and *React*.  
It provides a single interface for interacting with multiple standalone microservices located inside the `/modules` directory.  
Each microservice is a complete project (Node.js, Python, REST, gRPC, WebSocket, etc.), and the dashboard orchestrates them through a modern, responsive UI.

---

## **✨ Features**

- **Unified Dashboard** — One UI for all microservices  
- **Modular Architecture** — Each microservice lives in `/modules/<ServiceName>`  
- **Protocol‑Agnostic** — Supports REST, gRPC, WebSocket, SignalR, Python, Node.js  
- **Service Discovery** — Driven by `config/services.json`  
- **Real‑Time Streaming** — WebSocket & SignalR support  
- **Developer Tools** — Logs, request builders, inspectors  
- **Fully Responsive** — Built with Next.js App Router  

---

## **📁 Project Structure**

```
Utility/
│   package.json
│   README.md
│
├── app/                     # Next.js App Router (UI)
│   ├── (modules)/           # UI pages for each microservice
│   │   ├── bypasscors/
│   │   └── ipservice/
│   └── ...
│
├── config/
│   └── services.json        # Service registry for dashboard
│
├── lib/
│   └── loadServices.ts      # Service loader + route mapping
│
├── modules/                 # Each microservice is a full standalone project
│   ├── BypassCorsService/   # Node.js REST microservice
│   └── IPService/           # Python gRPC microservice
│
└── styles/                  # Global styles

```

🔌 Supported Microservice Types

    gRPC Services

    REST Services

    WebSocket Streams

    SignalR Hubs

    Python Microservices

    Node Microservices

## **🔌 Microservices**

All backend services live inside the `/MicroserviceModules` directory.  
Each one is a complete, independent project with its own:

- Source code  
- Dependencies  
- Server  
- Dashboard (legacy)  
- Clients  
- Storage  
- Protos (for gRPC services)  

### **Included Services**

#### **1. IPService (Python, gRPC)**  
Located at: `modules/IPService/`  
Implements IP lookup, BGP data, geo‑location, and more.
Generation code from root:
npx grpc_tools_node_protoc --js_out=import_style=commonjs,binary:lib/grpc/generated --grpc_out=grpc_js:lib/grpc/generated -I lib/grpc/protos lib/grpc/protos/ip_service.proto

#### **2. BypassCorsService (Node.js, REST)**  
Located at: `modules/BypassCorsService/`  
Implements a CORS‑bypass proxy with logging and reporting.


🧩 Adding a New Microservice Module

    Add its configuration to config/services.json

    Create a folder under app/modules/<ServiceName>

    Add protocol clients under lib or src/grpc

    Build UI components for testing, logs, and monitoring

## **⚙️ Service Discovery**

The dashboard reads all service definitions from:

config/services.json

name — Display name in dashboard

address — Host/port of the microservice

proto — gRPC proto file (empty for REST services)

🖥️ Dashboard (Next.js)

The dashboard lives in:

app/

The dashboard automatically picks up new modules.

It provides:

    Service list

    Per‑service pages

    REST request builder

    gRPC proxy calls

    Live streaming views

    Logs and diagnostics

🛠 Development

npm install
npm run dev

Start microservices

Each service inside /modules/<ServiceName> has its own README and startup instructions.

📜 License

MIT — free to use, modify, and extend.