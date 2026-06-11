"use client";

import { useState } from "react";

export default function BypassCorsPage() {
  const [url, setUrl] = useState("");
  const [result, setResult] = useState<any>(null);
  const [error, setError] = useState("");

  const sendRequest = async () => {
    setError("");
    setResult(null);

    if (!url.trim()) {
      setError("Please enter a target URL.");
      return;
    }

    try {
      const res = await fetch(`/api/bypasscors`, {
        method: "POST",
        body: JSON.stringify({ url })
      });

      const json = await res.json();

      if (json.error) setError(json.error);
      else setResult(json);
    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <div style={{ maxWidth: "900px" }}>
      <h1 style={{ marginBottom: "12px" }}>🛡️ BypassCorsService — CORS Proxy</h1>

      <p style={{ opacity: 0.7, marginBottom: "20px" }}>
        A production‑ready CORS bypass proxy that forwards HTTP(S) requests to any
        target URL while injecting permissive CORS headers. Includes logging,
        PostgreSQL persistence, analytics dashboard, and Swagger documentation.
      </p>

      <div style={{ marginBottom: "20px" }}>
        <input
          value={url}
          onChange={(e) => setUrl(e.target.value)}
          placeholder="https://example.com/api"
          style={{
            width: "100%",
            padding: "10px",
            borderRadius: "6px",
            border: "1px solid #334155",
            background: "#0f172a",
            color: "white"
          }}
        />

        <p style={{ fontSize: "12px", color: "#64748b", marginTop: "6px" }}>
          Enter any URL to proxy.  
          Example: <strong>https://example.com</strong>  
          The service will request it as:  
          <strong>http://localhost:8080/https://example.com</strong>
        </p>

        <button
          onClick={sendRequest}
          style={{
            marginTop: "12px",
            padding: "8px 16px",
            background: "#3b82f6",
            border: "none",
            borderRadius: "6px",
            cursor: "pointer"
          }}
        >
          Send Request
        </button>
      </div>

      {error && (
        <div style={{ color: "red", marginBottom: "20px" }}>
          <strong>Error:</strong> {error}
        </div>
      )}

      {result && (
        <pre
          style={{
            background: "#1e293b",
            padding: "16px",
            borderRadius: "8px",
            border: "1px solid #334155",
            overflowX: "auto"
          }}
        >
          {JSON.stringify(result, null, 2)}
        </pre>
      )}

      <h2 style={{ marginTop: "40px" }}>📘 Usage Examples</h2>

      <pre
        style={{
          background: "#0f172a",
          padding: "12px",
          borderRadius: "6px",
          overflowX: "auto"
        }}
      >
{`http://localhost:8080/https://example.com`}
      </pre>

      <h2>⚡ Auto‑CORS Snippet (Vanilla JS)</h2>
      <pre
        style={{
          background: "#0f172a",
          padding: "12px",
          borderRadius: "6px",
          overflowX: "auto"
        }}
      >
{`(function() {
  var cors_api_host = 'localhost:8080';
  var cors_api_url = 'http://' + cors_api_host + '/';
  var slice = [].slice;
  var origin = window.location.protocol + '//' + window.location.host;
  var open = XMLHttpRequest.prototype.open;

  XMLHttpRequest.prototype.open = function() {
    var args = slice.call(arguments);
    var targetOrigin = /^https?:\\/\\/([^\\/]+)/i.exec(args[1]);

    if (targetOrigin && targetOrigin[0].toLowerCase() !== origin &&
        targetOrigin[1] !== cors_api_host) {
      args[1] = cors_api_url + args[1];
    }

    return open.apply(this, args);
  };
})();`}
      </pre>

      <h2>⚡ Auto‑CORS Snippet (jQuery)</h2>
      <pre
        style={{
          background: "#0f172a",
          padding: "12px",
          borderRadius: "6px",
          overflowX: "auto"
        }}
      >
{`jQuery.ajaxPrefilter(function(options) {
  if (options.crossDomain && jQuery.support.cors) {
    options.url = 'http://localhost:8080/' + options.url;
  }
});`}
      </pre>

      <h2>📊 Dashboard</h2>
      <ul>
        <li><a href="http://localhost:8080/dashboard-ui">Dashboard UI</a></li>
        <li><a href="http://localhost:8080/dashboard/stats">/dashboard/stats</a></li>
        <li><a href="http://localhost:8080/dashboard/recent">/dashboard/recent</a></li>
      </ul>

      <h2>📘 Swagger / API Docs</h2>
      <ul>
        <li><a href="http://localhost:8080/docs">Swagger UI</a></li>
        <li><a href="http://localhost:8080/swagger.yaml">swagger.yaml</a></li>
      </ul>
    </div>
  );
}
