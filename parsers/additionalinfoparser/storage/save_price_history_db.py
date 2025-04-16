import psycopg2

def save_price_history(price_history_dict, db_config):
    conn = None
    try:
        conn = psycopg2.connect(**db_config)
        cursor = conn.cursor()
        total_entries = 0

        for flat_id, history in price_history_dict.items():
            if not history:
                continue

            for entry in history:
                cursor.execute(
                    """
                    INSERT INTO "PriceHistories" ("FlatId", "Price", "ChangeDate")
                    VALUES (%s, %s, %s)
                    """,
                    (flat_id, entry['price'], entry['date'])
                )

            total_entries += len(history)
            print(f"  Добавлено {len(history)} записей для квартиры {flat_id}")

        conn.commit()
        print(f"Всего обновлено {len(price_history_dict)} квартир, добавлено {total_entries} записей")

    except Exception as e:
        print(f"Ошибка: {e}")
        if conn:
            conn.rollback()
    finally:
        if conn:
            conn.close()