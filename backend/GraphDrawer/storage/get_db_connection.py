import psycopg2
from os import getenv
from dotenv import load_dotenv


def get_db_connection():
    load_dotenv()
    db_params = {
        "dbname": getenv("DB_NAME"),
        "user": getenv("DB_USER"),
        "password": getenv("DB_PASSWORD"),
        "host": getenv("DB_HOST"),
        "port": getenv("DB_PORT"),
    }

    return psycopg2.connect(**db_params)