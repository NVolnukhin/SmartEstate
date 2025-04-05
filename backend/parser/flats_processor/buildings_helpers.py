from geocoder_yandex import geocode_yandex
import csv
from datetime import datetime
from storage.buildings_db_storage import find_developer_id

def get_unique_buildings_info():
    """Собирает уникальные данные о зданиях из CSV."""
    building_data = set()

    with open('../flats_parse_result.csv', 'r', encoding='utf-8') as file:
        reader = csv.reader(file, delimiter=';')
        next(reader)
        for row in reader:
            if row:
                building_info = (
                    row[7].strip(),
                    f"г. {row[3]}, {row[20].strip()}, {row[21].strip()}",  # address
                    row[0],
                    row[11],
                    row[23]
                )
                building_data.add(building_info)

    return building_data


def get_construction_status(year: int) -> str:
    current_year = datetime.now().year
    if int(year) < current_year:
        return "построено"
    elif int(year) > current_year:
        return "строится"
    else:
        return "в процессе завершения"


def use_geocode_and_find_additional_info(buildings_set: set, db_params) -> list:
    """Геокодирует адреса зданий и находит более подробную информацию."""
    geocoded_data = []

    for i, (floors_count, address, developer, year, residential_complex) in enumerate(buildings_set, 1):
        coordinates = geocode_yandex(address)

        if not coordinates:
            print(f"Ошибка геокодирования для адреса: {address}")
            continue

        developer_id = find_developer_id(developer, db_params)
        status = get_construction_status(year)

        geocoded_data.append({
            'floors_count': floors_count,
            'address': address,
            'developer_id': developer_id,
            'status': status,
            'lat': coordinates[1],
            'lon': coordinates[0],
            'residential_complex': residential_complex
        })

        print(f"geocoded {i} bilding")

    return geocoded_data
