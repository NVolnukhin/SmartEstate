from metroparser import parse_wiki_metro_stations
from storage.file_storage import save_to_csv
from storage.db_storage import save_to_postgresql
from os import getenv
from dotenv import load_dotenv

def main():
    urls = {
        "https://ru.wikipedia.org/wiki/Список_станций_Московского_метрополитена",
        "https://ru.wikipedia.org/wiki/%D0%9D%D0%B5%D0%BA%D1%80%D0%B0%D1%81%D0%BE%D0%B2%D1%81%D0%BA%D0%B0%D1%8F_%D0%BB%D0%B8%D0%BD%D0%B8%D1%8F",
        "https://ru.wikipedia.org/wiki/%D0%A2%D1%80%D0%BE%D0%B8%D1%86%D0%BA%D0%B0%D1%8F_%D0%BB%D0%B8%D0%BD%D0%B8%D1%8F",
        "https://ru.wikipedia.org/wiki/%D0%9C%D0%BE%D1%81%D0%BA%D0%BE%D0%B2%D1%81%D0%BA%D0%BE%D0%B5_%D1%86%D0%B5%D0%BD%D1%82%D1%80%D0%B0%D0%BB%D1%8C%D0%BD%D0%BE%D0%B5_%D0%BA%D0%BE%D0%BB%D1%8C%D1%86%D0%BE"}

    load_dotenv()
    db_params = {
        "dbname": getenv("DB_NAME"),
        "user": getenv("DB_USER"),
        "password": getenv("DB_PASSWORD"),
        "host": getenv("DB_HOST"),
        "port": getenv("DB_PORT"),
    }

    for url in urls:
        print(url)
        stations = parse_wiki_metro_stations(url)
        if not stations:
            print("Не удалось получить данные станций")

        print(f"\nНайдено станций: {len(stations)}\n")

        save_to_postgresql(stations, db_params)


if __name__ == "__main__":
    main()

# /opt/anaconda3/bin/python3 /Users/nikitavolnuhin/ghostbusters/parsers/metroparser/main.py