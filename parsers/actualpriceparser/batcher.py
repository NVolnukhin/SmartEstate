import psycopg2
from get_actual_price import get_cian_price

def process_flats_batch(offset, db_params):
    """Обрабатывает партию из 10 квартир"""
    conn = None
    try:
        conn = psycopg2.connect(**db_params)
        cur = conn.cursor()

        cur.execute(f"""
            SELECT "FlatId", "CianLink" 
            FROM "Flats" 
            ORDER BY "FlatId" 
            OFFSET {offset} LIMIT 10
        """)
        flats = cur.fetchall()

        for flat_id, cian_link in flats:
            price = get_cian_price(cian_link)
            if price is not None:
                cur.execute("""
                    INSERT INTO "PriceHistories" ("FlatId", "Price", "ChangeDate")
                    VALUES (%s, %s, NOW() AT TIME ZONE 'UTC')
                """, (flat_id, price))
                print(f"Добавлено в БД: flat id - {flat_id}     price - {price}")
            else:
                print(f"Не удалось получить цену для квартиры {flat_id}")

        conn.commit()
        return len(flats)

    except Exception as e:
        print(f"Ошибка при обработке партии {offset}: {str(e)}")
        if conn:
            conn.rollback()
        return 0
    finally:
        if conn:
            conn.close()

