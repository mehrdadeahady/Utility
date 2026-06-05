🛰️ IPService — gRPC‑Powered IP Intelligence Service

    IPService is a lightweight, modular backend service that provides
    client IP detection, GeoIP lookup, and BGP network intelligence  
    through a high‑performance gRPC API.

✨ Features

    Client IP Detection — Extracts real client IP from gRPC metadata

    Geo Lookup — Country, city, coordinates, ISP, organization

    BGP Intelligence — ASN, prefix, holder, RIR, allocation metadata

    Parallel Execution — GEO + BGP lookups run concurrently

    Typed Responses — Clean, structured protobuf messages

    Modular Architecture — Easy to extend with new lookup modules

📚 Tech Stack

Python
gRPC
Protocol Buffers
Concurrent Futures
Dataclasses

📦 Project Structure

```
IPService/
├── client.py
├── server.py
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
│   ├── models.py
│   └── __pycache__/
│
├── protos/
│   └── ip_service.proto
│
└── server/
    ├── main.py
    ├── ip_service.py
    ├── geo_service.py
    ├── bgp_service.py
    └── __init__.py

```

▶️ Running the Server

python server.py

Server listens on:

localhost:50051

Returns:

    Your IP

    Geo information

    BGP information

🛠️ Regenerating Protobuf Files:

python -m grpc_tools.protoc -I protos --python_out=. --grpc_python_out=. protos/ip_service.proto
