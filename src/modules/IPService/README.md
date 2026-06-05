🛰️ IPService — gRPC‑Powered IP Intelligence Service

    IPService is a lightweight, modular backend service that provides
    client IP detection, GeoIP lookup, and BGP network intelligence  
    through a high‑performance gRPC API.

    The service also includes persistent request logging, a SQLite database,
    and a fully modular Flask dashboard for viewing historical IP lookups.

✨ Features

    Client IP Detection — Extracts real client IP from gRPC metadata

    Geo Lookup — Country, city, coordinates, ISP, organization

    BGP Intelligence — ASN, prefix, holder, RIR, allocation metadata

    Parallel Execution — GEO + BGP lookups run concurrently

    Typed Responses — Clean, structured protobuf messages

    Modular Architecture — Easy to extend with new lookup modules

    SQLite Database Logging — Every request + result stored automatically

    Server‑Side Logging — Human‑readable logs for each request

    Dashboard UI — Beautiful Flask dashboard to browse logs

    Search, Pagination, Dark Mode — Built‑in dashboard enhancements

📚 Tech Stack

Python
gRPC
Protocol Buffers
Concurrent Futures
SQLite
Flask (Dashboard)
Bootstrap 5 (UI)

📦 Project Structure

```
IPService/
├── client.py
├── server.py
├── reports.py                # Launches the dashboard server
├── requirements.txt
│
├── clients/
│   ├── test_client.py
│   └── __init__.py
│
├── generated/
│   ├── ip_service_pb2.py
│   ├── ip_service_pb2_grpc.py
│   └── __init__.py
│
├── models/
│   ├── models.py             # GeoModel, BgpModel, IpModel
│   └── __pycache__/
│
├── protos/
│   └── ip_service.proto
│
├── server/
│   ├── main.py               # gRPC server entrypoint
│   ├── ip_service.py         # Core service implementation
│   ├── geo_service.py        # Geo lookup logic
│   ├── bgp_service.py        # BGP lookup logic
│   └── __init__.py
│
├── storage/
│   ├── ipservice.db          # SQLite database
│   ├── storage.py            # Request + result persistence
│   └── __pycache__/
│
└── dashboard/
    ├── server/
    │   └── app.py            # Flask dashboard server
    │
    ├── templates/
    │   └── index.html        # Dashboard UI
    │
    └── static/
        ├── style.css         # Light mode
        └── dark.css          # Dark mode
```

▶️ Running the Server

python server.py

Server listens on:

localhost:50051

Each request returns:

    Client IP

    Geo information

    BGP information

All results are automatically:

    Logged to console

    Saved into storage/ipservice.db

📊 Running the Dashboard (Flask)

Start the dashboard:

python reports.py

Open in browser:

http://localhost:8080

Dashboard Features

    Fully responsive Bootstrap UI

    Search (IP, country, ASN, ISP, holder, prefix)

    Pagination

    Dark mode toggle (saved via localStorage)

    Real‑time view of all logged requests

🛠️ Regenerating Protobuf Files:

python -m grpc_tools.protoc -I protos --python_out=. --grpc_python_out=. protos/ip_service.proto

🗄️ Database Schema (SQLite)
requests table

| Column | Type | Description |
| --- | --- | --- |
| id | INTEGER | Primary key |
| client_ip | TEXT | Extracted client IP |
| created_at | DATETIME | Timestamp |

results table

| Column | Type | Description |
| --- | --- | --- |
| request_id | INTEGER | FK → requests.id |
| geo_country | TEXT | Country name |
| geo_city | TEXT | City |
| geo_isp | TEXT | ISP |
| bgp_asn | INTEGER | ASN |
| bgp_holder | TEXT | ASN holder |
| bgp_prefix | TEXT | BGP prefix |
| ... | ... | Additional metadata |

📝 Logging

Every request produces:

[REQUEST] Client IP: 91.23.44.10
[RESULT] GEO: {...}
[RESULT] BGP: {...}
[DB] Saved request_id=12

