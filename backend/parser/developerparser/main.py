import developersparser
from storage.file_storage import save_to_csv
from storage.db_storage import save_to_postgresql

def main():
    # Получаем данные
    developers = developersparser.get_developers_data()

    # Сохраняем в CSV
    save_to_csv(developers)

    db_params = {
        'dbname': 'smartestatedb',
        'user': 'postgres',
        'password': '123',
        'host': 'localhost',
        'port': '5432'
    }
    save_to_postgresql(developers, db_params)

    print(f"Готово! Сохранено данных по {len(developers)} застройщикам")

if __name__ == "__main__":
    main()