from os import getenv
import requests
from dotenv import load_dotenv


def geocode_yandex(address):
    """
    Геокодирует адрес через Яндекс.Геокодер и возвращает [долгота, широта].

    :param address: Строка с адресом
    :param api_key: Ваш API-ключ Яндекс.Карт (задается в .env)
    :return: Массив [долгота, широта] или None, если адрес не найден
    """
    try:
        load_dotenv()
        API_KEY = getenv("YANDEX_API_KEY")
        response = requests.get(
            "https://geocode-maps.yandex.ru/1.x/",
            params={
                "apikey": API_KEY,
                "geocode": address,
                "format": "json",
            }
        ).json()

        pos = response["response"]["GeoObjectCollection"]["featureMember"][0]["GeoObject"]["Point"]["pos"]
        lon, lat = map(float, pos.split())
        return [lon, lat]

    except (KeyError, IndexError, requests.RequestException):
        return None