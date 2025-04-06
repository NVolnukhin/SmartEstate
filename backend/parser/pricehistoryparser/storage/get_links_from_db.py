from get_db_connection import get_db_connection
from psycopg2 import sql

def get_cian_links_from_db():
    conn = get_db_connection()
    try:
        with conn.cursor() as cursor:
            query = sql.SQL("""
            SELECT "FlatId", "CianLink" FROM "Flats"
            """)
            cursor.execute(query)
            return cursor.fetchall()
    finally:
        conn.close()
