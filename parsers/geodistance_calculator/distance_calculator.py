from typing import Tuple
from storage.buildings_db_storage import get_building_coordinates
from storage.nearest_object import find_nearest_object
from os import getenv


def get_infrastructure_info(db_params, building_id: int) -> Tuple[int, int, int, int, int]:
    """Возвращает ID ближайших объектов инфраструктуры"""

    home_lat, home_lon = get_building_coordinates(db_params, building_id)
    print(f"Координаты здания {building_id}: Широта = {home_lat}, Долгота = {home_lon}")

    print("Поиск ближайших объектов...\n")

    kg_id, kg_dist = find_nearest_object(db_params, home_lat, home_lon, getenv("DB_KINDERGARTEN_TABLE_NAME"))
    school_id, school_dist = find_nearest_object(db_params, home_lat, home_lon, getenv("DB_SCHOOL_TABLE_NAME"))
    shop_id, shop_dist = find_nearest_object(db_params, home_lat, home_lon, getenv("DB_SHOP_TABLE_NAME"))
    pharmacy_id, pharmacy_dist = find_nearest_object(db_params, home_lat, home_lon, getenv("DB_PHARMACY_TABLE_NAME"))
    metro_id, metro_dist = find_nearest_object(db_params, home_lat, home_lon, getenv("DB_METRO_TABLE_NAME"))

    print(f"Ближайший детский сад (ID: {kg_id}) - {kg_dist:.2f} км")
    print(f"Ближайшая школа (ID: {school_id}) - {school_dist:.2f} км")
    print(f"Ближайший магазин (ID: {shop_id}) - {shop_dist:.2f} км")
    print(f"Ближайшая аптека (ID: {pharmacy_id}) - {pharmacy_dist:.2f} км")
    print(f"Ближайшее метро (ID: {metro_id}) - {metro_dist:.2f} км\n")

    return kg_id, school_id, shop_id, pharmacy_id, metro_id