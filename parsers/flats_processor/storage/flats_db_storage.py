import psycopg2
from psycopg2 import sql


def save_flats_to_postgresql(flats, db_params):
    """Сохраняет данные квартир в PostgreSQL."""
    try:
        conn = psycopg2.connect(**db_params)
        cursor = conn.cursor()

        # create_table_query = """
        #     CREATE TABLE IF NOT EXISTS "Flats" (
        #         "FlatId" INTEGER GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
        #         "Square" NUMERIC NOT NULL,
        #         "Roominess" INTEGER NOT NULL,
        #         "Floor" INTEGER NOT NULL,
        #         "CianLink" TEXT NOT NULL,
        #         "BuildingId" INTEGER NOT NULL,
        #         "FinishType" TEXT NOT NULL
        #     )
        # """
        # cursor.execute(create_table_query)
        #
        # cursor.execute('TRUNCATE TABLE public."Flats" RESTART IDENTITY CASCADE')

        insert_query = sql.SQL("""
            INSERT INTO "Flats" (
                "Square", 
                "Roominess", 
                "Floor", 
                "CianLink", 
                "BuildingId", 
                "FinishType"
            ) VALUES (%s, %s, %s, %s, %s, %s)
        """)

        # Вставка данных
        for flat in flats:
            cursor.execute(insert_query, (
                flat['square'],
                flat['roominess'],
                flat['floor'],
                flat['cian_link'],
                flat['building_id'],
                flat['finish_type']
            ))

        conn.commit()
        print(f"Успешно сохранено {len(flats)} квартир в БД PostgreSQL")
        return True

    except Exception as e:
        print(f"Ошибка при сохранении квартир в PostgreSQL: {e}")
        return False
    finally:
        if conn:
            conn.close()


def find_building_id(address: str, db_params) -> int:
    """Ищет ID здания по адресу базе данных."""

    query = sql.SQL("""
        SELECT "BuildingId" 
        FROM "Buildings" 
        WHERE "Address" LIKE %s
        LIMIT 1
    """)

    try:
        with psycopg2.connect(**db_params) as conn:
            with conn.cursor() as cursor:
                cursor.execute(query, (f"%{address}%",))
                result = cursor.fetchone()
                return result[0] if result else None

    except psycopg2.Error as e:
        print(f"Ошибка при поиске застройщика: {e}")
        raise




# f"г. {row[3]}, {row[20].strip()}, {row[21].strip()}",  # address