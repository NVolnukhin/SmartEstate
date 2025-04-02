import psycopg2
from psycopg2 import sql


def save_to_postgresql(stations, db_params):
    """Сохраняет данные станций в PostgreSQL."""
    try:
        conn = psycopg2.connect(**db_params)
        cursor = conn.cursor()

        create_table_query = """
        CREATE TABLE IF NOT EXISTS Metro (
            id SERIAL PRIMARY KEY,
            metro_station VARCHAR(100) NOT NULL,
            latitude NUMERIC(9,6) NOT NULL,
            longitude NUMERIC(9,6) NOT NULL
        )
        """
        cursor.execute(create_table_query)

        cursor.execute("TRUNCATE TABLE Metro RESTART IDENTITY")

        insert_query = sql.SQL("""
        INSERT INTO Metro (metro_station, latitude, longitude)
        VALUES (%s, %s, %s)
        """)

        for station in stations:
            cursor.execute(insert_query, (
                station['metro_station'],
                station['latitude'],
                station['longitude']
            ))

        conn.commit()
        print(f"Успешно сохранено {len(stations)} станций в БД PostgreSQL")
        return True

    except Exception as e:
        print(f"Ошибка при сохранении в PostgreSQL: {e}")
        return False
    finally:
        if conn:
            conn.close()