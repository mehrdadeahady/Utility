import { NextRequest } from "next/server";
import { getService } from "../../../lib/loadServices";

export async function POST(req: NextRequest) {
  try {
    const { path, method, headers, body } = await req.json();

    const svc = getService("BypassCorsService");
    if (!svc) {
      return Response.json({ error: "Service not found" }, { status: 404 });
    }

    const url = `http://${svc.address}${path}`;

    const response = await fetch(url, {
      method,
      headers,
      body: method !== "GET" && method !== "HEAD" ? body : undefined
    });

    const text = await response.text();

    return Response.json({
      ok: response.ok,
      status: response.status,
      statusText: response.statusText,
      body: text
    });
  } catch (err: any) {
    return Response.json({ error: err.message }, { status: 500 });
  }
}
