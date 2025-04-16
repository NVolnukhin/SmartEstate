import requests
from bs4 import BeautifulSoup


def get_cian_price(url):
    """Получает текущую цену квартиры с сайта Циан"""
    try:
        headers = {
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36'
        }
        response = requests.get(url, headers=headers, timeout=10)
        response.raise_for_status()

        soup = BeautifulSoup(response.text, 'html.parser')

        price_block = soup.find('div', {'data-testid': 'price-amount'})
        if price_block:
            price_element = price_block.find('span')
            if price_element:
                price_text = price_element.get_text(strip=True)
                price_text = ''.join(c if c.isdigit() else ' ' for c in price_text)
                return int(price_text.replace(' ', ''))

        print(f"Не удалось найти цену на странице: {url}")
        return None

    except Exception as e:
        print(f"Ошибка при получении цены для {url}: {str(e)}")
        return None