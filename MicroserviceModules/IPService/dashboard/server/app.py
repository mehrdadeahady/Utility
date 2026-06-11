from flask import Flask, render_template, request
import sqlite3
from pathlib import Path
import math
BASE_DIR = Path(__file__).resolve().parent.parent
DB_PATH = Path("storage/ipservice.db")

app = Flask(
    __name__,
    template_folder=str(BASE_DIR / "templates"),
    static_folder=str(BASE_DIR / "static")
)


def fetch_data(search="", page=1, per_page=20):
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    cur = conn.cursor()

    search_query = f"%{search}%"

    # Count total rows
    cur.execute("""
        SELECT COUNT(*)
        FROM requests r
        LEFT JOIN results res ON res.request_id = r.id
        WHERE r.client_ip LIKE ?
           OR res.geo_country LIKE ?
           OR res.geo_city LIKE ?
           OR res.geo_isp LIKE ?
           OR res.bgp_asn LIKE ?
           OR res.bgp_holder LIKE ?
           OR res.bgp_prefix LIKE ?
    """, (search_query,)*7)

    total = cur.fetchone()[0]
    pages = math.ceil(total / per_page)

    offset = (page - 1) * per_page

    cur.execute("""
        SELECT 
            r.id,
            r.client_ip,
            r.created_at,
            res.geo_country,
            res.geo_city,
            res.geo_country_code,
            res.geo_isp,
            res.bgp_asn,
            res.bgp_holder,
            res.bgp_prefix
        FROM requests r
        LEFT JOIN results res ON res.request_id = r.id
        WHERE r.client_ip LIKE ?
           OR res.geo_country LIKE ?
           OR res.geo_city LIKE ?
           OR res.geo_isp LIKE ?
           OR res.bgp_asn LIKE ?
           OR res.bgp_holder LIKE ?
           OR res.bgp_prefix LIKE ?
        ORDER BY r.id DESC
        LIMIT ? OFFSET ?
    """, (*([search_query] * 7), per_page, offset))

    rows = cur.fetchall()
    conn.close()

    return rows, pages


@app.get("/")
def index():
    search = request.args.get("search", "")
    page = int(request.args.get("page", 1))

    rows, pages = fetch_data(search, page)

    return render_template(
        "index.html",
        rows=rows,
        pages=pages,
        current_page=page,
        search=search
    )


if __name__ == "__main__":
    app.run(port=8080, debug=True)
