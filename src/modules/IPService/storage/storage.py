import sqlite3
from pathlib import Path

DB_PATH = Path("storage/ipservice.db")


class Storage:
    def __init__(self, path: Path = DB_PATH):
        self.path = path
        self._init_db()

    def _init_db(self):
        conn = sqlite3.connect(self.path)
        cur = conn.cursor()

        # Store each request
        cur.execute("""
        CREATE TABLE IF NOT EXISTS requests (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            client_ip TEXT,
            created_at DATETIME DEFAULT CURRENT_TIMESTAMP
        )
        """)

        # Store results for each request
        cur.execute("""
        CREATE TABLE IF NOT EXISTS results (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            request_id INTEGER,
            geo_country TEXT,
            geo_city TEXT,
            geo_lat REAL,
            geo_lon REAL,
            geo_isp TEXT,
            geo_org TEXT,
            geo_country_code TEXT,
            bgp_asn INTEGER,
            bgp_holder TEXT,
            bgp_prefix TEXT,
            bgp_rir TEXT,
            bgp_country TEXT,
            bgp_allocated TEXT,
            bgp_name TEXT,
            bgp_description TEXT,
            FOREIGN KEY(request_id) REFERENCES requests(id)
        )
        """)

        conn.commit()
        conn.close()

    # Save the request and return its ID
    def save_request(self, client_ip: str) -> int:
        conn = sqlite3.connect(self.path)
        cur = conn.cursor()
        cur.execute(
            "INSERT INTO requests (client_ip) VALUES (?)",
            (client_ip,)
        )
        request_id = cur.lastrowid
        conn.commit()
        conn.close()
        return request_id

    # Save the lookup result for that request
    def save_result(self, request_id: int, geo: dict, bgp: dict):
        conn = sqlite3.connect(self.path)
        cur = conn.cursor()

        cur.execute("""
        INSERT INTO results (
            request_id,
            geo_country, geo_city, geo_lat, geo_lon,
            geo_isp, geo_org, geo_country_code,
            bgp_asn, bgp_holder, bgp_prefix, bgp_rir,
            bgp_country, bgp_allocated, bgp_name, bgp_description
        ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """, (
            request_id,
            geo.get("country"),
            geo.get("city"),
            geo.get("latitude"),
            geo.get("longitude"),
            geo.get("isp"),
            geo.get("org"),
            geo.get("countryCode"),
            bgp.get("asn"),
            bgp.get("holder"),
            bgp.get("prefix"),
            bgp.get("rir"),
            bgp.get("country"),
            bgp.get("allocated"),
            bgp.get("name"),
            bgp.get("description"),
        ))

        conn.commit()
        conn.close()
