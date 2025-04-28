import math
import constants


def calculate_distance(lat1: float, lon1: float, lat2: float, lon2: float) -> float:
    lat1_rad = math.radians(lat1)
    lon1_rad = math.radians(lon1)
    lat2_rad = math.radians(lat2)
    lon2_rad = math.radians(lon2)

    delta_lon = lon2_rad - lon1_rad

    cos_d = (math.sin(lat1_rad) * math.sin(lat2_rad) + math.cos(lat1_rad) * math.cos(lat2_rad) * math.cos(delta_lon))

    cos_d = max(min(cos_d, 1.0), -1.0)

    distance_rad = math.acos(cos_d)
    return distance_rad * constants.EARTH_RADIUS