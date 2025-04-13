from batcher import process_flats_batch
import time
from os import getenv
from dotenv import load_dotenv

def main():
    load_dotenv()
    db_params = {
        "dbname": getenv("DB_NAME"),
        "user": getenv("DB_USER"),
        "password": getenv("DB_PASSWORD"),
        "host": getenv("DB_HOST"),
        "port": getenv("DB_PORT"),
    }

    total_processed = 0
    offset = 0

    while True:
        processed = process_flats_batch(offset, db_params)
        if processed == 0:
            break

        total_processed += processed
        offset += 10

        time.sleep(5)

    print(f"Обработка завершена. Всего обработано квартир: {total_processed}")


if __name__ == "__main__":
    main()