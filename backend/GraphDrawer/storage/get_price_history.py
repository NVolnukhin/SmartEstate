from get_db_connection import get_db_connection

def get_price_history(flat_id):
    conn = get_db_connection()
    try:
        with conn.cursor() as cursor:
            query = """
                SELECT "Price", "ChangeDate" 
                FROM "PriceHistories" 
                WHERE "FlatId" = %s
                ORDER BY "ChangeDate" ASC
            """
            cursor.execute(query, (flat_id,))
            result = cursor.fetchall()
            return [{'price': row[0], 'date': row[1]} for row in result]
    finally:
        conn.close()