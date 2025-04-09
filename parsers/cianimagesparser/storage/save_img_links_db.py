import psycopg2
from psycopg2.extras import Json


def overwrite_flats_images(images_dict, db_config):
    """
    Полностью перезаписывает поле Images для заданных ID.
    :param images_dict: {id: {"url1", "url2", ...}}
    :param db_config: {dbname, user, password, host, port}
    """
    conn = None
    try:
        conn = psycopg2.connect(**db_config)
        cursor = conn.cursor()

        for flat_id, urls in images_dict.items():
            cursor.execute(
                """
                UPDATE "Flats" 
                SET "Images" = %s
                WHERE "FlatId" = %s
                """,
                (Json(list(urls)), flat_id)  # Просто перезаписываем
            )

        conn.commit()
        print(f"Перезаписано {len(images_dict)} записей.")

    except Exception as e:
        print(f"Ошибка: {e}")
        if conn:
            conn.rollback()
    finally:
        if conn:
            conn.close()