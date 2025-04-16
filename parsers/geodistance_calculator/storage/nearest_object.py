from typing import Tuple
import psycopg2
from psycopg2 import sql
import sys
from pathlib import Path
sys.path.append(str(Path(__file__).parent.parent))
from coordinates_proccesing import calculate_distance


def find_nearest_object(db_params, home_lat: float, home_lon: float, table_name: str) -> Tuple[int, float]:
    """Находит ближайший объект заданного типа из БД"""
    conn = None
    try:
        conn = psycopg2.connect(**db_params)
        cursor = conn.cursor()

        query = sql.SQL("""
            SELECT "Id", "Latitude", "Longitude"
            FROM {}
            ORDER BY ST_Distance(
                ST_MakePoint(%s, %s)::geography,
                ST_MakePoint("Longitude", "Latitude")::geography
            )
            LIMIT 1
        """).format(sql.Identifier(table_name))

        cursor.execute(query, (home_lon, home_lat))
        obj_id, obj_lat, obj_lon = cursor.fetchone()

        distance = calculate_distance(home_lat, home_lon, float(obj_lat), float(obj_lon))
        return obj_id, distance

    except Exception as e:
        print(f"Ошибка при поиске ближайшего {table_name}: {e}")
        raise
    finally:
        if conn:
            conn.close()
