import type { ReactNode } from "react";
import "../styles/globals.css";
import { loadServices } from "../lib/loadServices";

export const metadata = {
  title: "Utility Dashboard",
  description: "Unified dashboard for multi-protocol microservices"
};

export default function RootLayout({ children }: { children: ReactNode }) {
  const services = loadServices();

  return (
    <html lang="en">
      <body>
        <div style={{ display: "flex", minHeight: "100vh" }}>
          <aside
            style={{
              width: "240px",
              background: "#1e293b",
              padding: "20px",
              borderRight: "1px solid #334155"
            }}
          >
            <h1 style={{ fontSize: "20px", marginBottom: "20px" }}>
              Utility Dashboard
            </h1>

            <nav style={{ display: "flex", flexDirection: "column", gap: "8px" }}>
              <a
                href="/"
                style={{
                  display: "block",
                  padding: "8px 12px",
                  color: "white",
                  textDecoration: "none",
                  borderRadius: "4px"
                }}
              >
                Overview
              </a>

             {services.map((svc) => (
              <a
                key={svc.name}
                href={`/${svc.route}`}
                style={{
                  display: "block",
                  padding: "8px 12px",
                  color: "white",
                  textDecoration: "none",
                  borderRadius: "4px"
                }}
              >
                {svc.name}
              </a>
            ))}

            </nav>

          </aside>

          <main style={{ flex: 1, padding: "24px" }}>{children}</main>
        </div>
      </body>
    </html>
  );
}
