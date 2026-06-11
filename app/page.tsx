import { loadServices } from "../lib/loadServices";

export default function HomePage() {
  const services = loadServices();

  return (
    <div>
      <h2 style={{ fontSize: "24px", marginBottom: "16px" }}>Services</h2>

      <div
        style={{
          display: "grid",
          gap: "16px",
          gridTemplateColumns: "repeat(auto-fill, minmax(240px, 1fr))"
        }}
      >
        {services.map((svc) => (
          <a
            key={svc.name}
            href={`/${svc.route}`}
            style={{
              display: "block",
              padding: "16px",
              border: "1px solid #334155",
              borderRadius: "8px",
              background: "#1e293b",
              textDecoration: "none",
              color: "white",
              cursor: "pointer"
            }}
          >
            <h3 style={{ margin: 0 }}>{svc.name}</h3>
            <p style={{ fontSize: "12px", opacity: 0.7 }}>{svc.address}</p>
            <p style={{ fontSize: "12px", opacity: 0.5 }}>
              {svc.proto ? "gRPC" : "REST"}
            </p>
          </a>
        ))}
      </div>
    </div>
  );
}
