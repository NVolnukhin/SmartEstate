import csv
from storage.flats_db_storage import find_building_id

def get_flats_info(db_params):
    """Собирает данные о квартирах из CSV."""
    flats_data = []

    with open('../flats_parse_result.csv', 'r', encoding='utf-8') as file:
        reader = csv.reader(file, delimiter=';')
        next(reader)
        for row in reader:
            if row:
                address = f"г. {row[3]}, {row[20].strip()}, {row[21].strip()}"
                building_id = find_building_id(address, db_params)

                flats_data.append({
                    'square': row[9],
                    'roominess': row[8],
                    'floor': row[6],
                    'cian_link': row[2],
                    'building_id': building_id,
                    'finish_type': row[15]
                })
                
                print("proceeded flat row")

    return flats_data


# f"г. {row[3]}, {row[20].strip()} {row[21].strip()}",  # address