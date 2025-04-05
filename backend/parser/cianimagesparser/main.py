from imgparser import parse_cian_images as pci
from os import getenv
from dotenv import load_dotenv
from storage.get_cian_urls_db import get_links as gl
from storage.save_img_links_db import overwrite_flats_images as ofi




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
    for idx, link in links_dict.items():
        print(f"{idx} - {link}")

    images_dict = pci({1: "https://www.cian.ru/sale/flat/298030201/",
                       2: "https://www.cian.ru/sale/flat/314441491/",
                       3: "https://www.cian.ru/sale/flat/313404545/"})



    for idx, imgs in images_dict.items():
        print(f"ID {idx}: {imgs}")

    ofi(images_dict, db_config)