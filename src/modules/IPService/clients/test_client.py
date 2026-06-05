import grpc
from generated import ip_service_pb2, ip_service_pb2_grpc

def serve():
    channel = grpc.insecure_channel("localhost:50051")
    stub = ip_service_pb2_grpc.IpServiceStub(channel)

    response = stub.GetClientIp(ip_service_pb2.IpRequest())

    print("Your IP:", response.ip)

    # GEO
    print("\n--- GEO INFO ---")
    print("Country:", response.geo.country)
    print("City:", response.geo.city)
    print("Latitude:", response.geo.latitude)
    print("Longitude:", response.geo.longitude)
    print("ASN:", response.geo.asn)
    print("Org:", response.geo.org)
    print("Country Code:", response.geo.countryCode)
    print("ISP:", response.geo.isp)
    print("AS Field:", response.geo.as_field)

    # BGP
    print("\n--- BGP INFO ---")
    print("ASN:", response.bgp.asn)
    print("Holder:", response.bgp.holder)
    print("Prefix:", response.bgp.prefix)
    print("RIR:", response.bgp.rir)
    print("Country:", response.bgp.country)
    print("Allocated:", response.bgp.allocated)
    print("Name:", response.bgp.name)
    print("Description:", response.bgp.description)


if __name__ == "__main__":
    serve()
