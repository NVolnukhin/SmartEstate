from os import getenv
from dotenv import load_dotenv
from .storage.infrastructure_info_db_storage import set_infrastructure_info

def process_infrastructure_info():
    load_dotenv()
    db_params = {
        "dbname": getenv("DB_NAME"),
        "user": getenv("DB_USER"),
        "password": getenv("DB_PASSWORD"),
        "host": getenv("DB_HOST"),
        "port": getenv("DB_PORT"),
    }

    set_infrastructure_info(db_params)