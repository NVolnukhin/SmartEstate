from bs4 import BeautifulSoup
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from datetime import datetime
import locale
import random
import time
import pickle
import os
USER_AGENTS = [
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36",
    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36"
]
import random

class CianParser:
    def __init__(self):
        self.driver = None
        self.cookies_file = "cian_cookies.pkl"
        self.setup_locale()
        self.user_agent = random.choice(USER_AGENTS)
        self.setup_driver()

    def setup_locale(self):
        """Устанавливаем русскую локаль для корректного парсинга дат"""
        try:
            locale.setlocale(locale.LC_TIME, 'ru_RU.UTF-8')
        except locale.Error:
            try:
                locale.setlocale(locale.LC_TIME, 'Russian_Russia.1251')
            except locale.Error:
                print("Warning: Не удалось установить русскую локаль. Даты могут парситься некорректно.")

    def setup_driver(self):
        """Настраиваем драйвер с анти-детект параметрами"""
        options = Options()

        options.add_argument("--disable-blink-features=AutomationControlled")
        options.add_experimental_option("excludeSwitches", ["enable-automation"])
        options.add_argument(f"user-agent={self.user_agent}")
        options.add_argument("--start-maximized")

        self.driver = webdriver.Chrome(options=options)
        self.load_cookies()

    def load_cookies(self):
        """Загружаем сохраненные куки для поддержания сессии"""
        if os.path.exists(self.cookies_file):
            try:
                self.driver.get("https://cian.ru")
                time.sleep(2)

                with open(self.cookies_file, 'rb') as f:
                    cookies = pickle.load(f)

                for cookie in cookies:
                    if 'expiry' in cookie:
                        del cookie['expiry']
                    self.driver.add_cookie(cookie)

                print("Успешно загружены куки из файла")
            except Exception as e:
                print(f"Ошибка загрузки кук: {str(e)}")

    def save_cookies(self):
        """Сохраняем текущие куки для следующего запуска"""
        try:
            cookies = self.driver.get_cookies()
            with open(self.cookies_file, 'wb') as f:
                pickle.dump(cookies, f)
            print("Куки успешно сохранены")
        except Exception as e:
            print(f"Ошибка сохранения кук: {str(e)}")

    def handle_captcha(self):
        """Обработка появления капчи"""
        if "captcha" in self.driver.current_url.lower():
            print("\n⚠️ Обнаружена CAPTCHA! Требуется ручное вмешательство.")
            print("1. Решите капчу в открывшемся браузере")
            print("2. После успешного решения нажмите Enter здесь...")
            input(">>> Нажмите Enter после решения капчи <<<")
            return True
        return False

    def parse_single_page(self, url):
        """Парсим данные с одной страницы"""
        try:

            time.sleep(random.uniform(1, 3))

            self.driver.get(url)
            time.sleep(random.uniform(2, 4))

            if self.handle_captcha():
                time.sleep(5)
                self.driver.get(url)
                time.sleep(random.uniform(2, 4))

            soup = BeautifulSoup(self.driver.page_source, 'html.parser')
            images = self.parse_images(soup)
            price_history = self.parse_price_history(soup)

            return images, price_history

        except Exception as e:
            print(f"Ошибка парсинга страницы: {str(e)}")
            return set(), []

    def parse_images(self, soup):
        """Извлекаем все изображения со страницы"""
        images = set()

        try:
            main_img = soup.find('li', class_='a10a3f92e9--container--Havpv')
            if main_img and (img := main_img.find('img')) and img.get('src'):
                images.add(img['src'])

            floor_plan = soup.find('div', {'data-name': 'FloorPlan'})
            if floor_plan and (img := floor_plan.find('img')) and img.get('src'):
                images.add(img['src'])

            for div in soup.find_all('div', class_='a10a3f92e9--container--zARIJ'):
                if img := div.find('img'):
                    if src := img.get('src'):
                        images.add(src)

        except Exception as e:
            print(f"Ошибка парсинга изображений: {str(e)}")

        return images

    def parse_price_history(self, soup):
        """Извлекаем историю цен"""
        history = []

        try:
            for row in soup.find_all('tr', class_='a10a3f92e9--history-event--xUQ_P'):
                date_elem = row.find('td', class_='a10a3f92e9--event-date--BvijC')
                price_elem = row.find('td', class_='a10a3f92e9--event-price--xNv2v')

                if not (date_elem and price_elem):
                    continue

                try:
                    date_text = date_elem.get_text(strip=True)
                    parsed_date = datetime.strptime(date_text, '%d %b %Y')

                    price_text = price_elem.get_text(strip=True)
                    price_value = int(''.join(filter(str.isdigit, price_text)))

                    history.append({
                        'date': parsed_date,
                        'price': price_value
                    })
                except ValueError:
                    continue

        except Exception as e:
            print(f"Ошибка парсинга истории цен: {str(e)}")

        return history

    def close(self):
        """Корректное закрытие драйвера с сохранением сессии"""
        try:
            if self.driver:
                self.save_cookies()
                self.driver.quit()
        except Exception as e:
            print(f"Ошибка при закрытии драйвера: {str(e)}")
        finally:
            self.driver = None