from .parser_engine import CianParser
from os import getenv
from dotenv import load_dotenv
from .storage.get_cian_urls_db import get_links as gl, get_null_img_links as glnull
from .storage.save_img_links_db import overwrite_flats_images as ofi
from .storage.save_price_history_db import save_price_history as sph
import time

BATCH_SIZE = 1000


def parse_additional_info():
    load_dotenv()
    db_config = {
        "dbname": getenv("DB_NAME"),
        "user": getenv("DB_USER"),
        "password": getenv("DB_PASSWORD"),
        "host": getenv("DB_HOST"),
        "port": getenv("DB_PORT"),
    }

    links_dict = glnull(db_config)
    total_flats = len(links_dict)
    processed_flats = 0

    print(f"Найдено {total_flats} квартир для обработки")
    parser = CianParser()

    try:
        batch_links = dict(list(links_dict.items())[:BATCH_SIZE])

        for flat_id, url in batch_links.items():
            try:
                print(f"\nОбрабатываю квартиру ID: {flat_id}")

                images, price_history = parser.parse_single_page(url)

                if "captcha" in parser.driver.current_url.lower():
                    print("Обнаружена капча! Требуется ручное вмешательство.")
                    input("Решите капчу в браузере и нажмите Enter для продолжения...")
                    images, price_history = parser.parse_single_page(url)

                if images:
                    ofi({flat_id: images}, db_config)
                    print(f"Сохранено {len(images)} изображений")

                if price_history:
                    sph({flat_id: price_history}, db_config)
                    print(f"Сохранено {len(price_history)} записей истории цен")

                processed_flats += 1
                print(
                    f"Прогресс: {processed_flats}/{min(BATCH_SIZE, total_flats)} ({processed_flats / min(BATCH_SIZE, total_flats):.1%})")

                time.sleep(2)
                if (processed_flats == 1):
                    time.sleep(10) # time for login to parse price history

            except Exception as e:
                print(f"Ошибка при обработке квартиры {flat_id}: {e}")
                continue

        print("\nОбработка батча завершена!")
    finally:
        parser.close()