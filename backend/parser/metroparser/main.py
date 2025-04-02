from metroparser import parse_wiki_metro_stations
from storage.file_storage import save_to_csv
from storage.db_storage import save_to_postgresql


def main():
    stations = parse_wiki_metro_stations()
    if not stations:
        print("Не удалось получить данные станций")
        return

    print(f"\nНайдено станций: {len(stations)}\n")

    # Сохраняем в файл
    save_to_csv(stations)

    # Сохраняем в PostgreSQL
    db_params = {
        'dbname': 'smartestatedb',
        'user': 'postgres',
        'password': '123',
        'host': 'localhost',
        'port': '5432'
    }
    save_to_postgresql(stations, db_params)


if __name__ == "__main__":
    main()

# /opt/anaconda3/bin/python3 /Users/nikitavolnuhin/ghostbusters/backend/parser/metroparser/main.py