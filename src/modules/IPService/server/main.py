import grpc
from concurrent import futures

from generated import ip_service_pb2_grpc
from server.ip_service import IpService


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    ip_service_pb2_grpc.add_IpServiceServicer_to_server(IpService(), server)

    server.add_insecure_port("[::]:50051")
    print("gRPC IP server running on port 50051")
    server.start()
    server.wait_for_termination()


if __name__ == "__main__":
    serve()
