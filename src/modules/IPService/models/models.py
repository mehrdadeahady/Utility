from dataclasses import dataclass
from typing import Optional, Dict, Any, List


# -----------------------------
# GEO MODEL
# -----------------------------
@dataclass
class GeoModel:
    country: Optional[str] = None
    city: Optional[str] = None
    latitude: Optional[float] = None
    longitude: Optional[float] = None
    asn: Optional[str] = None
    org: Optional[str] = None
    countryCode: Optional[str] = None
    isp: Optional[str] = None
    as_field: Optional[str] = None  # "as" is a reserved keyword in Python

# -----------------------------
# BGP MODEL
# -----------------------------
@dataclass
class BgpModel:
    asn: Optional[int] = None
    holder: Optional[str] = None
    prefix: Optional[str] = None
    rir: Optional[str] = None
    country: Optional[str] = None
    allocated: Optional[str] = None
    name: Optional[str] = None
    description: Optional[str] = None

# -----------------------------
# IP MODEL (FINAL COMBINED RESULT)
# -----------------------------
@dataclass
class IpModel:
    ip: Optional[str]
    geo: Optional[GeoModel]
    bgp: Optional[BgpModel]
