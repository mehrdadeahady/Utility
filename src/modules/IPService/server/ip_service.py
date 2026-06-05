import grpc
import urllib.parse
import concurrent.futures
from generated import ip_service_pb2, ip_service_pb2_grpc
from server.geo_service import geo_lookup
from server.bgp_service import bgp_lookup
from models.models import GeoModel, BgpModel, IpModel

import grpc
import urllib.parse
import concurrent.futures

from generated import ip_service_pb2, ip_service_pb2_grpc

from server.geo_service import geo_lookup
from server.bgp_service import bgp_lookup

from models.models import GeoModel, BgpModel, IpModel


class IpService(ip_service_pb2_grpc.IpServiceServicer):

    def GetClientIp(self, request, context):
        peer = context.peer()
        decoded = urllib.parse.unquote(peer)

        # Extract IP
        if decoded.startswith("ipv6:["):
            ip = decoded.split("]")[0].replace("ipv6:[", "")
        elif decoded.startswith("ipv4:"):
            ip = decoded.split(":")[1]
        else:
            ip = decoded

        # ------------------------------------
        # 1) CHECK IF IP EXISTS
        # ------------------------------------
        if not ip or ip.strip() == "":
            # Return empty response immediately
            return ip_service_pb2.IpResponse(
                ip="",
                geo="{}",
                bgp="{}"
            )

        # ------------------------------------
        # 2) RUN BOTH SERVICES IN PARALLEL
        # ------------------------------------
        with concurrent.futures.ThreadPoolExecutor() as pool:
            future_geo = pool.submit(geo_lookup, ip)
            future_bgp = pool.submit(bgp_lookup, ip)

            geo_data = future_geo.result()
            bgp_data = future_bgp.result()

        # ------------------------------------
        # 3) CONVERT TO MODELS
        # ------------------------------------
        geo_model = GeoModel(
            country=geo_data.get("country"),
            city=geo_data.get("city"),
            latitude=geo_data.get("latitude"),
            longitude=geo_data.get("longitude"),
            asn=geo_data.get("asn"),
            org=geo_data.get("org"),
            countryCode=geo_data.get("countryCode"),
            isp=geo_data.get("isp"),
            as_field=geo_data.get("as")
        )

        bgp_model = BgpModel(
            asn=bgp_data.get("asn"),
            holder=bgp_data.get("holder"),
            prefix=bgp_data.get("prefix"),
            rir=bgp_data.get("rir"),
            country=bgp_data.get("country"),
            allocated=bgp_data.get("allocated"),
            name=bgp_data.get("name"),
            description=bgp_data.get("description")
        )

        result = IpModel(
            ip=ip,
            geo=geo_model,
            bgp=bgp_model
        )

        # ------------------------------------
        # 4) RETURN COMBINED RESULT
        # ------------------------------------
        return ip_service_pb2.IpResponse(
            ip=result.ip,
            geo=ip_service_pb2.Geo(
                country=result.geo.country,
                city=result.geo.city,
                latitude=result.geo.latitude,
                longitude=result.geo.longitude,
                asn=result.geo.asn,
                org=result.geo.org,
                countryCode=result.geo.countryCode,
                isp=result.geo.isp,
                as_field=result.geo.as_field
            ),
            bgp=ip_service_pb2.Bgp(
                asn=result.bgp.asn,
                holder=result.bgp.holder,
                prefix=result.bgp.prefix,
                rir=result.bgp.rir,
                country=result.bgp.country,
                allocated=result.bgp.allocated,
                name=result.bgp.name,
                description=result.bgp.description
            )
        )
