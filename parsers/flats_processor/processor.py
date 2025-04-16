from .buildings_helpers import get_unique_buildings_info, use_geocode_and_find_additional_info
from .flats_helper import get_flats_info
from .storage.buildings_db_storage import save_buildings_to_postgresql
from .storage.flats_db_storage import save_flats_to_postgresql

def proceed_buildings(db_params):
    buildings = get_unique_buildings_info()
    geo_buildings = use_geocode_and_find_additional_info(buildings, db_params)
    
    for i, building in enumerate(geo_buildings, 1):
        print(f"{i} - {building}")

    save_buildings_to_postgresql(geo_buildings, db_params)

def proceed_flats(db_params):
    flats = get_flats_info(db_params)
    for i, flat in enumerate(flats, 1):
        print(f"{i} - {flat}")
    save_flats_to_postgresql(flats, db_params)

    
def process_records(db_params):
    proceed_buildings(db_params)
    proceed_flats(db_params)