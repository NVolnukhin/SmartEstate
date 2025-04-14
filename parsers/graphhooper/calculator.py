from itertools import cycle

import psycopg2
import requests
from typing import Dict, Optional
import time
from .location import Location
import random
from os import getenv
from dotenv import load_dotenv


class WalkingTimeCalculator:
    def __init__(self):
        """
        Инициализация калькулятора времени пути.
        """

        load_dotenv()
        db_config = {
            "dbname": getenv("DB_NAME"),
            "user": getenv("DB_USER"),
            "password": getenv("DB_PASSWORD"),
            "host": getenv("DB_HOST"),
            "port": getenv("DB_PORT"),
        }
        self.db_config = db_config
        self.base_url = "https://graphhopper.com/api/1/route"
        self.conn = None
        self.key = cycle([f"GRAPH_HOOPER_API_KEY_{i}" for i in range(1, 8)])

    def connect_db(self):
        """Установка соединения с БД"""
        try:
            self.conn = psycopg2.connect(**self.db_config)
            print("Подключение к БД установлено")
            return True
        except Exception as e:
            print(f"Ошибка подключения: {e}")
            return False

    def disconnect_db(self):
        """Закрытие соединения с БД"""
        if self.conn:
            self.conn.close()
            print("Соединение с БД закрыто")

    def get_coordinates(self, table: str, obj_id: int) -> Optional[Location]:
        """Получение координат объекта из указанной таблицы"""
        query = f"""
        SELECT {table}."Id", {table}."Latitude", {table}."Longitude" FROM {table} WHERE {table}."Id" = %s;
        """
        try:
            with self.conn.cursor() as cursor:
                cursor.execute(query, (obj_id,))
                result = cursor.fetchone()
                if result:
                    return Location(id=result[0], latitude=result[1], longitude=result[2])
                return None
        except Exception as e:
            print(f"Ошибка при получении координат: {e}")
            return None

    def calculate_walking_time(self, start: Location, end: Location) -> Optional[int]:
        """
        Расчет времени пешего пути между точками

        :param start: Начальная точка (здание)
        :param end: Конечная точка (объект инфраструктуры)
        :return: Время в минутах или None при ошибке
        """
        params = {
            'point': [f"{start.latitude},{start.longitude}", f"{end.latitude},{end.longitude}"],
            'vehicle': 'foot',
            'key': getenv(next(self.key)),
            'calc_points': False
        }

        print(params['key'])

        try:
            response = requests.get(self.base_url, params=params, timeout=10)
            response.raise_for_status()
            data = response.json()
            return int(data['paths'][0]['time'] / 60 / 1000)  # мс в мин
        except Exception as e:
            print(f"Ошибка API для маршрута {start.id}->{end.id}: {e}")
            return None

    def update_walking_times(self, building_id: int, updates: Dict[str, int]):
        """
        Обновление времени пути в БД

        :param building_id: ID здания
        :param updates: Словарь {поле_времени: минуты}
        """
        set_clause = ", ".join([f"{field} = %s" for field in updates.keys()])
        values = list(updates.values()) + [building_id]

        query = f"""
        UPDATE "InfrastructureInfos" 
        SET {set_clause}
        WHERE "BuildingId" = %s;
        """

        try:
            with self.conn.cursor() as cursor:
                cursor.execute(query, values)
                self.conn.commit()
                print(f"Обновлено здание {building_id}: {updates}")
        except Exception as e:
            print(f"Ошибка при обновлении здания {building_id}: {e}")
            self.conn.rollback()

    def process_buildings(self, batch_size: int = 131):
        """
        Основной метод обработки зданий

        :param batch_size: Количество зданий для обработки за один раз
        """
        if not self.connect_db():
            return

        try:
            with self.conn.cursor() as cursor:
                cursor.execute("""
                SELECT COUNT(*) FROM "InfrastructureInfos";
                """)
                total = cursor.fetchone()[0]
                print(f"Всего зданий для обработки: {total}")

                for offset in range(0, total, batch_size):
                    print(f"\nОбработка зданий {offset + 1}-{min(offset + batch_size, total)}...")

                    query = """
                    SELECT 
                        i."BuildingId",
                        b."Latitude" as building_lat,
                        b."Longitude" as building_lon,
                        i."NearestKindergartenId",
                        i."NearestSchoolId", 
                        i."NearestMetroId",
                        i."NearestPharmacyId",
                        i."NearestShopId"
                    FROM "InfrastructureInfos" i
                    JOIN "Buildings" b ON i."BuildingId" = b."BuildingId"
                    ORDER BY i."BuildingId"
                    LIMIT %s OFFSET %s;
                    """
                    cursor.execute(query, (batch_size, offset))
                    batch = cursor.fetchall()

                    for row in batch:
                        building_id = row[0]
                        building_loc = Location(id=building_id, latitude=row[1], longitude=row[2])
                        updates = {}

                        infra_mapping = [
                            ('"NearestKindergartenId"', '"Kindergarten"', '"MinutesToKindergarten"'),
                            ('"NearestSchoolId"', '"School"', '"MinutesToSchool"'),
                            ('"NearestMetroId"', '"Metro"', '"MinutesToMetro"'),
                            ('"NearestPharmacyId"', '"Pharmacy"', '"MinutesToPharmacy"'),
                            ('"NearestShopId"', '"Shop"', '"MinutesToShop"')
                        ]

                        for id_field, table, time_field in infra_mapping:
                            infra_id = row[3 + infra_mapping.index((id_field, table, time_field))]
                            if not infra_id:
                                continue

                            infra_loc = self.get_coordinates(table, infra_id)
                            if not infra_loc:
                                continue

                            walking_time = self.calculate_walking_time(building_loc, infra_loc)
                            if time:
                                updates[time_field] = walking_time
                                time.sleep(1)

                        if updates:
                            self.update_walking_times(building_id, updates)

        except Exception as e:
            print(f"Критическая ошибка: {e}")
        finally:
            self.disconnect_db()