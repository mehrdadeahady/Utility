import grpc from "@grpc/grpc-js";
import { IpRequest } from "./generated/ip_service_pb";
import { IpServiceClient } from "./generated/ip_service_grpc_pb";

export function createIpServiceClient() {
  return new IpServiceClient(
    "localhost:50051",
    grpc.credentials.createInsecure()
  );
}

export function getClientIp(client: InstanceType<typeof IpServiceClient>,ip: string) {
  return new Promise((resolve, reject) => {
    const req = new IpRequest();
    req.setIp(ip);   // <-- THIS is the missing piece

    client.getClientIp(req, (err, response) => {
      if (err) reject(err);
      else resolve(response.toObject());
    });
  });
}
