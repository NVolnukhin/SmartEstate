import json
import csv
import os
import tkinter as tk
from tkinter import filedialog
from typing import Dict, List, Optional, Tuple


def ensure_secondary_data_dir() -> str:
    os.makedirs('secondary_data', exist_ok=True)
    return os.path.abspath('secondary_data')


def select_input_file() -> Optional[str]:
    root = tk.Tk()
    root.withdraw()
    file_path = filedialog.askopenfilename(
        title="Выберите GeoJSON файл",
        filetypes=[("GeoJSON files", "*.geojson;*.json"), ("All files", "*.*")]
    )
    return file_path if file_path else None


def get_user_input(prompt: str, default: str = "") -> str:
    user_input = input(prompt).strip()
    return user_input if user_input else default


def extract_coordinates(geometry: Dict) -> Optional[Tuple[float, float]]:
    if not geometry:
        return None

    geometry_type = geometry.get('type')
    coordinates = geometry.get('coordinates', [])

    if geometry_type == 'Point' and len(coordinates) >= 2:
        return (coordinates[1], coordinates[0])
    elif geometry_type in ['Polygon', 'LineString'] and coordinates:
        first_coords = coordinates[0] if geometry_type == 'Polygon' else coordinates
        if first_coords and len(first_coords[0]) >= 2:
            return (first_coords[0][1], first_coords[0][0])

    return None


def find_name_field(properties: Dict) -> Optional[str]:
    possible_name_fields = ['name', 'title', 'label', 'description', 'nom', 'nombre']

    for field in possible_name_fields:
        if field in properties:
            return field

    for field, value in properties.items():
        if isinstance(value, str) and value.strip():
            return field

    return None


def process_geojson_features(features: List[Dict]) -> Tuple[List[Dict], Optional[str]]:
    csv_data = []
    name_field = None

    for idx, feature in enumerate(features, start=1):
        properties = feature.get('properties', {})
        geometry = feature.get('geometry', {})

        if idx == 1:
            name_field = find_name_field(properties)
            if not name_field:
                print("В файле не найдено подходящего поля с именем")
                return [], None

        if not name_field or name_field not in properties or not properties[name_field]:
            continue

        name = properties[name_field]
        coords = extract_coordinates(geometry)

        if coords:
            latitude, longitude = coords
            csv_data.append({
                'ID': idx,
                'name': name,
                'latitude': latitude,
                'longitude': longitude
            })

    return csv_data, name_field


def write_to_csv(data: List[Dict], output_path: str):
    if not data:
        print("Нет данных для сохранения.")
        return

    try:
        with open(output_path, 'w', newline='', encoding='utf-8') as f:
            writer = csv.DictWriter(f, fieldnames=['ID', 'name', 'latitude', 'longitude'])
            writer.writeheader()
            writer.writerows(data)
        print(f"Файл успешно сохранен: {output_path}")
    except IOError as e:
        print(f"Ошибка при записи файла: {str(e)}")


def load_geojson(file_path: str) -> Optional[Dict]:
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            return json.load(f)
    except (IOError, json.JSONDecodeError) as e:
        print(f"Ошибка при чтении файла: {str(e)}")
        return None


def main():
    output_dir = ensure_secondary_data_dir()
    print(f"Результаты будут сохранены в: {output_dir}")

    input_file = select_input_file()
    if not input_file:
        print("Файл не выбран. Операция отменена.")
        return

    output_filename = get_user_input(
        "Введите имя выходного CSV файла (без расширения): ",
        default="output"
    ) + ".csv"

    output_path = os.path.join(output_dir, output_filename)

    geojson_data = load_geojson(input_file)
    if not geojson_data:
        return

    features = geojson_data.get('features', [])
    csv_data, name_field = process_geojson_features(features)

    if not name_field:
        return

    write_to_csv(csv_data, output_path)


if __name__ == "__main__":
    main()