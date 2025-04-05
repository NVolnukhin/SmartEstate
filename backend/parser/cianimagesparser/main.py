from imgparser import parse_cian_images as pci
from os import getenv
from dotenv import load_dotenv
from storage.get_cian_urls_db import get_links as gl
from storage.save_img_links_db import overwrite_flats_images as ofi
import time
BATCH_SIZE = 5

if __name__ == "__main__":
    load_dotenv()
    db_config = {
        "dbname": getenv("DB_NAME"),
        "user": getenv("DB_USER"),
        "password": getenv("DB_PASSWORD"),
        "host": getenv("DB_HOST"),
        "port": getenv("DB_PORT"),
    }

    links_dict = gl(db_config)
    total_flats = len(links_dict)
    processed_flats = 0

    while processed_flats < total_flats:
        batch_links = dict(list(links_dict.items())[processed_flats:processed_flats + BATCH_SIZE])
        images_dict = pci(batch_links)

        for idx, imgs in images_dict.items():
            print(f"ID {idx}: {imgs}")

        ofi(images_dict, db_config)
        processed_flats += len(batch_links)
        print(f"Обработано {processed_flats} из {total_flats} квартир")
        time.sleep(2)