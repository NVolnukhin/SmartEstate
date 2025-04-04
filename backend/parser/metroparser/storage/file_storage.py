import csv


def save_to_csv(stations, filename=None):
    """Сохраняет данные станций в CSV файл."""
    if not filename:
        filename = f"metro_parse_result.csv"

    try:
        with open(filename, 'w', newline='', encoding='utf-8') as csvfile:
            fieldnames = ['num', 'metro_station', 'latitude', 'longitude']
            writer = csv.DictWriter(csvfile, fieldnames=fieldnames)

            writer.writeheader()
            for idx, station in enumerate(stations, start=1):
                writer.writerow({
                    'ID': idx,
                    'name': station['metro_station'],
                    'latitude': station['latitude'],
                    'longitude': station['longitude']
                })
        print(f"Данные успешно сохранены в файл: {filename}")
        return True
    except Exception as e:
        print(f"Ошибка при сохранении в файл: {e}")
        return False