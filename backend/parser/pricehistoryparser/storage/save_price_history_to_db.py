from get_db_connection import get_db_connection
from psycopg2 import sql


def save_price_history_to_db(flat_id, price_history):
    conn = get_db_connection()
    try:
        with conn.cursor() as cursor:
            for entry in price_history:
                query = sql.SQL("""
                    INSERT INTO "PriceHistories" ("FlatId", "Price", "ChangeDate")
                    VALUES (%s, %s, %s)
                    ON CONFLICT ("FlatId", "ChangeDate") DO NOTHING
                """)
                cursor.execute(query, (flat_id, entry['price'], entry['date']))
        conn.commit()
    except Exception as e:
        print(f"Ошибка при сохранении в БД: {e}")
        conn.rollback()
    finally:
        conn.close()
