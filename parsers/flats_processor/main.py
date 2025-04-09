import buildings_helpers as bh
from os import getenv
from dotenv import load_dotenv

from flats_helper import get_flats_info
from storage.buildings_db_storage import save_buildings_to_postgresql
from storage.flats_db_storage import save_flats_to_postgresql

def proceed_buildings(db_params):
    buildings = bh.get_unique_buildings_info()
    geo_buildings = bh.use_geocode_and_find_additional_info(buildings, db_params)
    
    for i, building in enumerate(geo_buildings, 1):
        print(f"{i} - {building}")
    
    save_buildings_to_postgresql(geo_buildings, db_params)
    
def proceed_flats(db_params):
    flats = get_flats_info(db_params)
    for i, flat in enumerate(flats, 1):
        print(f"{i} - {flat}")
    save_flats_to_postgresql(flats, db_params)

    
def main():
    load_dotenv()
    db_params = {
        "dbname": getenv("DB_NAME"),
        "user": getenv("DB_USER"),
        "password": getenv("DB_PASSWORD"),
        "host": getenv("DB_HOST"),
        "port": getenv("DB_PORT"),
    }

    proceed_buildings(db_params)
    proceed_flats(db_params)


if __name__ == "__main__":
    main()