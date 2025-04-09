from storage.get_links_from_db import get_cian_links_from_db
from storage.save_price_history_to_db import save_price_history_to_db
from price_history_parser import parse_cian_price_history
import time


def main():
    cian_links = get_cian_links_from_db()

    if not cian_links:
        print("Не найдено ссылок на объявления в базе данных")
        return

    print(f"Найдено {len(cian_links)} объявлений для обработки")

    for flat_id, cian_link in cian_links:
        print(f"Обрабатываю объявление {flat_id}: {cian_link}")

        price_history = parse_cian_price_history(cian_link)
        print(price_history)
        if price_history:
            print(f"Найдено {len(price_history)} записей истории цен")
            save_price_history_to_db(flat_id, price_history)

        time.sleep(2)


if __name__ == "__main__":
    main()