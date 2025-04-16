import requests
from bs4 import BeautifulSoup
import time

headers = {
    'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36'
}


def get_developers_data(pages=95):
    base_url = "https://www.cian.ru/zastroishchiki/"
    developers_data = []

    for page in range(1, pages + 1):
        url = f"{base_url}?p={page}"
        try:
            response = requests.get(url, headers=headers)
            soup = BeautifulSoup(response.text, 'html.parser')

            rows = soup.find_all('tr', class_='catalog__row js-catalog-row')

            for row in rows:
                try:
                    link_tag = row.find('a', class_='catalog__cell-content')
                    if link_tag:
                        relative_link = link_tag['href']
                        full_link = f"https://www.cian.ru{relative_link}"

                        name = row.get_text().split('\n')[13].strip()

                        houses_tag = row.find('a', class_='catalog__cell__link', target='_blank')
                        builded_count = houses_tag.get_text(strip=True, separator=' ')
                        print(f"Link: {full_link} name: {name} Построено: {builded_count}")

                        developers_data.append({
                            'name': name,
                            'website': full_link,
                            'buildings': builded_count
                        })

                except Exception as e:
                    print(f"Ошибка при обработке строки: {e}")
                    continue

            print(f"Обработана страница {page}, найдено {len(rows)} застройщиков")
            time.sleep(2)

        except Exception as e:
            print(f"Ошибка на странице {page}: {e}")
            continue

    return developers_data