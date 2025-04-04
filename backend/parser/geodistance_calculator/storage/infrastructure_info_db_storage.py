from os import getenv
from dotenv import load_dotenv
import sys
from pathlib import Path
import psycopg2
from psycopg2 import sql
sys.path.append(str(Path(__file__).parent.parent))
from distance_calculator import get_infrastructure_info


def set_infrastructure_info(db_params):
    """
    Сохраняет ID ближайших объектов инфраструктуры для всех зданий

    :param db_params: Параметры подключения к БД
    """
    conn = None
    try:
        # Подключаемся к БД
        conn = psycopg2.connect(**db_params)
        cursor = conn.cursor()

        truncate_query = sql.SQL("""
        TRUNCATE TABLE "InfrastructureInfos" RESTART IDENTITY
        """)
        cursor.execute(truncate_query)

        # Получаем список всех ID зданий
        cursor.execute("""
        SELECT "BuildingId" FROM "Buildings"
        """)
        building_ids = [row[0] for row in cursor.fetchall()]

        for building_id in building_ids:
            # Получаем ID ближайших объектов
            print(f"Info for building ID = {building_id}")
            kg_id, school_id, shop_id, pharmacy_id, metro_id = get_infrastructure_info(db_params, building_id)
            print(f"{kg_id}, {school_id}, {shop_id}, {pharmacy_id}, {metro_id}")


            # Формируем и выполняем SQL-запрос
            query = sql.SQL("""
                INSERT INTO "InfrastructureInfos" (
                    "BuildingId",
                    "NearestShopId",
                    "NearestMetroId",
                    "NearestSchoolId",
                    "NearestKindergartenId",
                    "NearestPharmacyId"
                ) VALUES (
                    %s, %s, %s, %s, %s, %s
                )
                ON CONFLICT ("BuildingId") 
                DO UPDATE SET
                    "NearestShopId" = EXCLUDED."NearestShopId",
                    "NearestMetroId" = EXCLUDED."NearestMetroId",
                    "NearestSchoolId" = EXCLUDED."NearestSchoolId",
                    "NearestKindergartenId" = EXCLUDED. "NearestKindergartenId",
                    "NearestPharmacyId" = EXCLUDED."NearestPharmacyId"
            """)

            cursor.execute(query, (
                building_id,
                shop_id,
                metro_id,
                school_id,
                kg_id,
                pharmacy_id
            ))

        # Фиксируем изменения
        conn.commit()
        print(f"Успешно обновлена информация для {len(building_ids)} зданий")

    except Exception as e:
        print(f"Ошибка: {e}")
        if conn:
            conn.rollback()
    finally:
        if conn:
            conn.close()



def main():
    load_dotenv()
    db_params = {
        "dbname": getenv("DB_NAME"),
        "user": getenv("DB_USER"),
        "password": getenv("DB_PASSWORD"),
        "host": getenv("DB_HOST"),
        "port": getenv("DB_PORT"),
    }

    set_infrastructure_info(db_params)

if __name__ == "__main__":
    main()