import requests

def geo_lookup(ip: str):
    """Return geo + ISP info for a public IP using ip-api.com with ipapi.co fallback."""

    EMPTY = {
        "country": None,
        "city": None,
        "latitude": None,
        "longitude": None,
        "asn": None,
        "org": None,
        "countryCode": None,
        "isp": None,
        "as": None
    }

    # No IP (timeout hop)
    if not ip:
        return EMPTY

    # Private IPv4 ranges
    private_prefixes_v4 = (
        "10.",
        "192.168.",
        "172.16.", "172.17.", "172.18.", "172.19.",
        "172.20.", "172.21.", "172.22.", "172.23.",
        "172.24.", "172.25.", "172.26.", "172.27.",
        "172.28.", "172.29.", "172.30.", "172.31.",
        "127."
    )

    # Private IPv6 ranges
    private_prefixes_v6 = (
        "::1",
        "fc00:", "fd00:",  # Unique local
        "fe80:"            # Link-local
    )

    # Skip private IPs
    if ip.startswith(private_prefixes_v4) or ip.startswith(private_prefixes_v6):
        return EMPTY

    # -------------------------------
    # PRIMARY LOOKUP → ip-api.com
    # -------------------------------
    try:
        url = f"http://ip-api.com/json/{ip}?fields=status,countryCode,isp,org,as,country,city,lat,lon"
        r = requests.get(url, timeout=4)
        data = r.json()

        if data.get("status") == "success":
            return {
                "country": data.get("country"),
                "city": data.get("city"),
                "latitude": data.get("lat"),
                "longitude": data.get("lon"),
                "asn": data.get("as"),
                "org": data.get("org"),
                "countryCode": data.get("countryCode"),
                "isp": data.get("isp"),
                "as": data.get("as")
            }

    except Exception:
        pass  # fall through to fallback

    # -------------------------------
    # FALLBACK LOOKUP → ipapi.co
    # -------------------------------
    try:
        r = requests.get(f"https://ipapi.co/{ip}/json/", timeout=4)
        data = r.json()

        return {
            "country": data.get("country_name"),
            "city": data.get("city"),
            "latitude": data.get("latitude"),
            "longitude": data.get("longitude"),
            "asn": data.get("asn"),
            "org": data.get("org"),
            "countryCode": data.get("country_code"),
            "isp": data.get("org"),  # ipapi.co uses "org" for ISP
            "as": data.get("asn")
        }

    except Exception:
        return EMPTY

'''
🌍 GEO (Geolocation Data)

GEO tells you where an IP is located in the real world.

It's basically the address book of the internet.
📍 What GEO includes (short)

    Country — where the IP is registered

    City — approximate location

    Latitude / Longitude — map coordinates

    ISP — the internet provider

    Org — the organization behind the IP

    ASN — the network ID (shared with BGP)

    Country Code — ISO code (DE, US, FR…)

These come from IP registries and commercial databases.
🎯 Why GEO matters

    Detect user region

    Apply local rules (GDPR, taxes, content)

    Fraud detection

    Security filtering

    Analytics

It's the human-location layer of your IP service.
'''