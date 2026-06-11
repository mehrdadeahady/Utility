import { NextRequest } from "next/server";
import { createIpServiceClient, getClientIp } from "@/lib/grpc/ipServiceClient";

export async function POST(req: NextRequest) {
  try {
    const { ip } = await req.json();

    const client = createIpServiceClient();
    const result = await getClientIp(client, ip);

    return Response.json(result);

  } catch (err: any) {
    return Response.json({ error: err.message }, { status: 500 });
  }
}
