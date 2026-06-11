"use client";

import { useState } from "react";

export default function IPServicePage() {
  const [ip, setIp] = useState("8.8.8.8");
  const [data, setData] = useState<any>(null);
  const [error, setError] = useState("");

  const lookup = async () => {
    setError("");
    setData(null);

    try {
      const res = await fetch("/api/ipservice", {
        method: "POST",
        body: JSON.stringify({ ip })
      });

      const json = await res.json();

      if (json.error) {
        setError(json.error);
      } else {
        setData(json);
      }
    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <div style={{ maxWidth: "800px" }}>
      <h1 style={{ marginBottom: "12px" }}>🛰️ IPService — gRPC IP Intelligence</h1>

      <p style={{ opacity: 0.7, marginBottom: "20px" }}>
        High‑performance gRPC service providing GeoIP, BGP, ASN, ISP, prefix and
        network intelligence — powered by Python, SQLite, and concurrent lookup modules.
      </p>

      <div style={{ display: "flex", gap: "12px", marginBottom: "20px" }}>
        <input
          value={ip}
          onChange={(e) => setIp(e.target.value)}
          placeholder="Enter IP address or leave empty"
          style={{
            padding: "8px",
            flex: 1,
            borderRadius: "6px",
            border: "1px solid #334155",
            background: "#0f172a",
            color: "white"
          }}
        />

      </div>

      {/* Help text inserted here */}
      <p style={{ fontSize: "12px", color: "#64748b", marginTop: "-12px", marginBottom: "20px" }}>
        Leave the field empty to lookup <strong>your own IP</strong>.  
        Enter an IP address if you want to lookup information for a specific target.
      </p>

      <button
        onClick={lookup}
        style={{
          padding: "8px 16px",
          background: "#3b82f6",
          border: "none",
          borderRadius: "6px",
          cursor: "pointer",
          marginBottom: "20px"
        }}
      >
        Lookup
      </button>

      {error && (
        <div style={{ color: "red", marginBottom: "20px" }}>
          <strong>Error:</strong> {error}
        </div>
      )}

      {data && (
        <div
          style={{
            padding: "20px",
            background: "#1e293b",
            borderRadius: "8px",
            border: "1px solid #334155"
          }}
        >
          <h2 style={{ marginTop: 0 }}>📡 Lookup Result</h2>

          <h3>Client IP</h3>
          <p>{data.client_ip || ip}</p>

          {data.geo && (
            <>
              <h3>🌍 Geo Information</h3>
              <ul>
                <li>Country: {data.geo.country}</li>
                <li>City: {data.geo.city}</li>
                <li>Latitude: {data.geo.latitude}</li>
                <li>Longitude: {data.geo.longitude}</li>
                <li>ISP: {data.geo.isp}</li>
                <li>Organization: {data.geo.organization}</li>
              </ul>
            </>
          )}

          {data.bgp && (
            <>
              <h3>🛰️ BGP / ASN Information</h3>
              <ul>
                <li>ASN: {data.bgp.asn}</li>
                <li>Prefix: {data.bgp.prefix}</li>
                <li>Holder: {data.bgp.holder}</li>
                <li>RIR: {data.bgp.rir}</li>
                <li>Allocation Date: {data.bgp.allocation_date}</li>
              </ul>
            </>
          )}

          <h3>🧪 Raw Response</h3>
          <pre
            style={{
              background: "#0f172a",
              padding: "12px",
              borderRadius: "6px",
              overflowX: "auto"
            }}
          >
            {JSON.stringify(data, null, 2)}
          </pre>
        </div>
      )}
    </div>
  );
}
