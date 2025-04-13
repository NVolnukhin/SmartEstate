import csv
from .storage.flats_db_storage import find_building_id


def get_flats_info(db_params):
    """Собирает данные о квартирах из CSV."""
    flats_data = []
    skipped_count = 0

    with open('/Users/nikitavolnuhin/ghostbusters/parsers/flats_parse_result.csv', 'r', encoding='utf-8') as file:
        reader = csv.reader(file, delimiter=';')

        for row_num, row in enumerate(reader, 1):
            if not row:
                continue

            try:
                address = f"г. {row[3]}, {row[20].strip()}, {row[21].strip()}"
                building_id = find_building_id(address, db_params)

                if building_id is None:
                    skipped_count += 1
                    print(f"Строка {row_num}: Пропущено - не найден building_id для адреса: {address}")
                    continue

                finish_type = row[15].strip() if row[15].strip() else '-'

                flats_data.append({
                    'square': row[9],
                    'roominess': row[8],
                    'floor': row[6],
                    'cian_link': row[2],
                    'building_id': building_id,
                    'finish_type': finish_type
                })

                print(f"Строка {row_num}: Обработано")

            except IndexError as e:
                print(f"Строка {row_num}: Ошибка - не хватает данных в строке: {e}")
                skipped_count += 1
            except Exception as e:
                print(f"Строка {row_num}: Неожиданная ошибка: {e}")
                skipped_count += 1

    print(f"\nИтог обработки:")
    print(f"Успешно обработано: {len(flats_data)}")
    print(f"Пропущено: {skipped_count}")

    return flats_data

# f"г. {row[3]}, {row[20].strip()} {row[21].strip()}",  # address