import os

EARTH_RADIUS = 6371.0  # Радиус Земли в км
DATA_DIR = "secondary_data"

FILE_PATHS = {
    'kindergarten': os.path.join(DATA_DIR, 'kindergarten_parse_result.csv'),
    'school': os.path.join(DATA_DIR, 'school_parse_result.csv'),
    'shop': os.path.join(DATA_DIR, 'shop_parse_result.csv'),
    'pharmacy': os.path.join(DATA_DIR, 'pharmacy_parse_result.csv')
}