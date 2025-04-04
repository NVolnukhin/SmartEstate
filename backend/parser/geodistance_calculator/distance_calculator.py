from typing import Tuple
import finding_nearest_objects as finder


def get_user_coordinates() -> Tuple[float, float]:
    print("Введите координаты вашего дома:")
    try:
        lat = float(input("Широта (например, 55.7558 для Москвы): ").strip())
        lon = float(input("Долгота (например, 37.6173 для Москвы): ").strip())
        return lat, lon
    except ValueError:
        print("Ошибка: введите числовые значения координат")
        return get_user_coordinates()


def main():
    home_lat, home_lon = get_user_coordinates()

    print("\nПоиск ближайших объектов...\n")

    kg_id, kg_dist, kg_name = finder.find_nearest_kindergarten(home_lat, home_lon)
    school_id, school_dist, school_name = finder.find_nearest_school(home_lat, home_lon)
    shop_id, shop_dist, shop_name = finder.find_nearest_shop(home_lat, home_lon)
    pharmacy_id, pharmacy_dist, pharmacy_name = finder.find_nearest_pharmacy(home_lat, home_lon)

    print(f"Ближайший детский сад: {kg_name} (ID: {kg_id}) - {kg_dist:.2f} км")
    print(f"Ближайшая школа: {school_name} (ID: {school_id}) - {school_dist:.2f} км")
    print(f"Ближайший магазин: {shop_name} (ID: {shop_id}) - {shop_dist:.2f} км")
    print(f"Ближайшая аптека: {pharmacy_name} (ID: {pharmacy_id}) - {pharmacy_dist:.2f} км")


if __name__ == "__main__":
    main()