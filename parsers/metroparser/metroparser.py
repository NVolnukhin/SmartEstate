import requests
from bs4 import BeautifulSoup


def dms_to_decimal(coord_str):
    """Конвертирует координаты из DMS в десятичные градусы."""
    parts = coord_str.replace('″', '').replace('′', ' ').replace('°', ' ').split()
    degrees = float(parts[0])
    minutes = float(parts[1])
    seconds = float(parts[2])
    direction = parts[3]

    decimal = degrees + minutes / 60 + seconds / 3600
    if direction in ['ю.ш.', 'з.д.', 'S', 'W']:
        decimal *= -1
    return decimal


def parse_wiki_metro_stations():
    """Парсит список станций московского метро из Википедии."""
    url = "https://ru.wikipedia.org/wiki/Список_станций_Московского_метрополитена"
    headers = {'User-Agent': 'Mozilla/5.0'}

    try:
        response = requests.get(url, headers=headers)
        response.raise_for_status()
        soup = BeautifulSoup(response.text, 'html.parser')

        tables = soup.find_all('table', {'class': 'standard'})
        if not tables:
            print("Таблицы со станциями не найдены")
            return []

        main_table = tables[0]
        stations = []

        for row in main_table.find_all('tr')[1:]:
            cols = row.find_all('td')
            if len(cols) < 7:
                continue

            name_link = cols[1].find('a')
            if not name_link:
                continue

            name = name_link.get_text(strip=True)
            if '(' in name:
                name = name.split('(')[0].strip()

            coords_cell = cols[-2]
            latitude = longitude = None

            map_link = coords_cell.find('a', {'class': 'mw-kartographer-maplink'})
            if map_link:
                try:
                    latitude = float(map_link.get('data-lat'))
                    longitude = float(map_link.get('data-lon'))
                except (ValueError, AttributeError):
                    pass

            if latitude is None or longitude is None:
                coords_span = coords_cell.find('span', {'class': 'geo'})
                if coords_span:
                    try:
                        lat, lon = coords_span.get_text().split(';')
                        latitude = float(lat.strip())
                        longitude = float(lon.strip())
                    except:
                        pass
                else:
                    coords_span = coords_cell.find('span', {'class': 'coordinates'})
                    if coords_span:
                        coords_text = coords_span.get_text(strip=True)
                        parts = coords_text.split()
                        if len(parts) >= 8:
                            try:
                                lat_str = ' '.join(parts[:4])
                                lon_str = ' '.join(parts[4:8])
                                latitude = dms_to_decimal(lat_str)
                                longitude = dms_to_decimal(lon_str)
                            except:
                                pass

            if latitude is not None and longitude is not None:
                stations.append({
                    'metro_station': name,
                    'latitude': round(latitude, 6),
                    'longitude': round(longitude, 6)
                })

        return stations

    except Exception as e:
        print(f"Ошибка при парсинге страницы: {e}")
        return []