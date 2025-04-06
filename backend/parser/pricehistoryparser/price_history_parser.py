import requests
from bs4 import BeautifulSoup
from datetime import datetime
import locale

def parse_cian_price_history(url):
    headers = {
        'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36'
    }

    try:
        locale.setlocale(locale.LC_TIME, 'ru_RU.UTF-8')
    except locale.Error:
        try:
            locale.setlocale(locale.LC_TIME, 'Russian_Russia.1251')
        except locale.Error:
            print("Не удалось установить русскую локаль. Даты могут парситься некорректно.")

    try:
        response = requests.get(url, headers=headers)
        response.raise_for_status()
    except requests.exceptions.RequestException as e:
        print(f"Ошибка при загрузке страницы {url}: {e}")
        return None

    soup = BeautifulSoup(response.text, 'html.parser')
    history_rows = soup.find_all('tr', class_='a10a3f92e9--history-event--xUQ_P')

    price_history = []

    for row in history_rows:
        date_element = row.find('td', class_='a10a3f92e9--event-date--BvijC')
        price_element = row.find('td', class_='a10a3f92e9--event-price--xNv2v')

        if date_element and price_element:
            date_text = date_element.get_text(strip=True)
            try:
                parsed_date = datetime.strptime(date_text, '%d %b %Y')
            except ValueError:
                print(f"Не удалось распарсить дату: {date_text}")
                continue

            price_text = price_element.get_text(strip=True)
            price_value = ''.join(filter(str.isdigit, price_text))

            if price_value:
                price_history.append({
                    'date': parsed_date,
                    'price': int(price_value)
                })

    return price_history
