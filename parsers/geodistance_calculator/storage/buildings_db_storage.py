import psycopg2
from typing import Tuple

def get_building_coordinates(db_params, flat_id) -> Tuple[float, float]:
    try:
        conn = psycopg2.connect(**db_params)
        cursor = conn.cursor()

        query = """
        SELECT "Latitude", "Longitude" 
        FROM "Buildings"
        WHERE "BuildingId" = %s
        """
        cursor.execute(query, (flat_id,))

        result = cursor.fetchone()

        if result:
            latitude, longitude = result
            return latitude, longitude
        else:
            print(f"Квартира с ID {flat_id} не найдена")

    except psycopg2.Error as e:
        print(f"Ошибка при работе с PostgreSQL: {e}")

    finally:
        if 'conn' in locals():
            conn.close()
