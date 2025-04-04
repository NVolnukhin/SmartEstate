from typing import Tuple
import csv
import coordinates_proccesing as proccessor
from constants import FILE_PATHS


def find_nearest_object(home_lat: float, home_lon: float, filepath: str) -> Tuple[int, float, str]:
    min_distance = float('inf')
    nearest_id = -1
    obj_name = ""

    try:
        with open(filepath, 'r', encoding='utf-8') as file:
            reader = csv.DictReader(file)
            for row in reader:
                try:
                    obj_lat = float(row['latitude'])
                    obj_lon = float(row['longitude'])
                    distance = proccessor.calculate_distance(home_lat, home_lon, obj_lat, obj_lon)

                    if distance < min_distance:
                        min_distance = distance
                        nearest_id = int(row['ID'])
                        obj_name = row.get('name', '')
                except (ValueError, KeyError) as e:
                    print(f"Ошибка в строке {row}: {str(e)}")
                    continue
    except FileNotFoundError:
        print(f"Файл {filepath} не найден")
    except Exception as e:
        print(f"Ошибка при чтении файла {filepath}: {str(e)}")

    return nearest_id, min_distance, obj_name

def find_nearest_kindergarten(home_lat: float, home_lon: float) -> Tuple[int, float, str]:
    return find_nearest_object(home_lat, home_lon, FILE_PATHS['kindergarten'])


def find_nearest_school(home_lat: float, home_lon: float) -> Tuple[int, float, str]:
    return find_nearest_object(home_lat, home_lon, FILE_PATHS['school'])


def find_nearest_shop(home_lat: float, home_lon: float) -> Tuple[int, float, str]:
    return find_nearest_object(home_lat, home_lon, FILE_PATHS['shop'])


def find_nearest_pharmacy(home_lat: float, home_lon: float) -> Tuple[int, float, str]:
    return find_nearest_object(home_lat, home_lon, FILE_PATHS['pharmacy'])