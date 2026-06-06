import requests
from typing import Optional, Dict, Any

def bgp_lookup(ip: str):
    """Return BGP/ASN info for a public IP using RIPEstat with BGPView fallback."""

    EMPTY = {
        "asn": None,
        "holder": None,
        "prefix": None,
        "rir": None,
        "country": None,
        "allocated": None,
        "name": None,
        "description": None
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

    # ---------------------------------------------------
    # PRIMARY LOOKUP → RIPEstat prefix-overview
    # ---------------------------------------------------
    try:
        url = f"https://stat.ripe.net/data/prefix-overview/data.json?resource={ip}"
        r = requests.get(url, timeout=5)
        data = r.json()

        d = data.get("data", {})
        asns = d.get("asns", [])

        if asns:
            asn_info = asns[0]

            return {
                "asn": asn_info.get("asn"),
                "holder": asn_info.get("holder"),
                "prefix": d.get("prefix"),
                "rir": d.get("rir"),
                "country": d.get("country"),
                "allocated": d.get("allocated"),
                "name": asn_info.get("name"),
                "description": asn_info.get("description")
            }

    except Exception:
        pass  # fall through to fallback

    # ---------------------------------------------------
    # FALLBACK LOOKUP → BGPView.io
    # ---------------------------------------------------
    try:
        r = requests.get(f"https://api.bgpview.io/ip/{ip}", timeout=5)
        data = r.json()

        if data.get("status") == "ok":
            prefixes = data.get("data", {}).get("prefixes", [])
            asns = data.get("data", {}).get("asns", [])

            prefix_info = prefixes[0] if prefixes else {}
            asn_info = asns[0] if asns else {}

            return {
                "asn": asn_info.get("asn"),
                "holder": asn_info.get("description_short"),
                "prefix": prefix_info.get("prefix"),
                "rir": prefix_info.get("rir_name"),
                "country": prefix_info.get("country_code"),
                "allocated": prefix_info.get("date_allocated"),
                "name": asn_info.get("name"),
                "description": asn_info.get("description")
            }

    except Exception:
        return EMPTY

    return EMPTY


'''
🌐 BGP = Border Gateway Protocol

BGP is the system that:

    Connects all ISPs together

    Decides how data travels across the internet

    Chooses the best path between networks

    Uses ASN numbers to identify networks

Think of it as the GPS of the internet, but for routing traffic between large networks (Autonomous Systems).
🧩 Why you see BGP in your IP service

When you run:

    bgp_lookup(ip)

You get information like:

    ASN (Autonomous System Number)

    Holder (who owns the network)

    Prefix (the IP block)

    RIR (RIPE, ARIN, APNIC…)

    Country

    Allocation date

    Network name

    Description

This is all BGP metadata.
'''
'''
🌐 BGP (Border Gateway Protocol)

The internet's inter-network routing system. It decides how data moves between large networks (ASNs).
It's basically the postal system for the entire internet.
🆔 ASN (Autonomous System Number)

A unique ID for each network on the internet (e.g., Google, Cloudflare, Deutsche Telekom).
Think of it like a license plate for networks.
🛰️ How BGP works (short)

    Each ASN announces which IP ranges it owns.

    Other ASNs learn these announcements.

    Routers choose the best path to reach any IP.

    Traffic flows across the internet following these paths.

⚠️ Why BGP matters

    If BGP breaks, the internet breaks.

    Misconfigurations cause BGP leaks.

    Attacks cause BGP hijacks (stealing traffic).

🧩 How it fits your service

Your service uses BGP data to show:

    ASN

    Prefix

    Holder

    RIR

    Country

    Allocation info

This is the network-level identity of the IP.
'''