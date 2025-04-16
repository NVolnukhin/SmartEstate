import developersparser
from storage.file_storage import save_to_csv
from storage.db_storage import save_to_postgresql
from os import getenv
from dotenv import load_dotenv

def parse_devs():
    # Получаем данные
    developers = developersparser.get_developers_data()

    # Сохраняем в CSV
    save_to_csv(developers)

    load_dotenv()
    db_params = {
        "dbname": getenv("DB_NAME"),
        "user": getenv("DB_USER"),
        "password": getenv("DB_PASSWORD"),
        "host": getenv("DB_HOST"),
        "port": getenv("DB_PORT"),
    }
    save_to_postgresql(developers, db_params)

    print(f"Готово! Сохранено данных по {len(developers)} застройщикам")

if __name__ == "__main__":
    main()