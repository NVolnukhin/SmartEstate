# SmartEstate GeoDistanceCalculator
Поиск ближайших социальных объектов к заданным координатам
## Описание
GeoDistanceCalculator - это инструмент на Python 3 (версии 3.8 и выше) для поиска ближайших социально-значимых объектов к заданным географическим координатам. Программа анализирует данные из CSV-файлов и находит:

- Детские сады
- Школы
- Магазины
- Аптеки

## Установка
```bash
pip install geodistance_calculator
```
## Настройка
Создайте папку secondary_data

Поместите в нее CSV-файлы с данными:

- Детские сады: kindergarten_parse_result.csv

- Школы: school_parse_result.csv

- Магазины: shop_parse_result.csv

- Аптеки: pharmacy_parse_result.csv


### Формат файлов:

```
ID,name,latitude,longitude
1,Детский сад "Солнышко",55.1234,37.1234
2,Школа №1,55.1235,37.1235
...
```
## Использование
### Базовый запуск
```bash
from geodistance_finder import main

main()
```
### Получение ближайших объектов

```bash
from geodistance_finder import (
    find_nearest_kindergarten,
    find_nearest_school,
)

# Пример
lat, lon = 55.7558, 37.6173  # Координаты Москвы (Красная площадь, 1)


kg_id, kg_dist, kg_name = finder.find_nearest_kindergarten(lat, lon)
school_id, school_dist, school_name = finder.find_nearest_school(lat, lon)
# Аналогично для магазинов и аптек
```
## Вывод результатов

#### Результаты выводятся в консоль в формате:
```
"Ближайший детский сад: {kg_name} (ID: {kg_id}) - {kg_dist:.2f} км"
"Ближайшая школа: {school_name} (ID: {school_id}) - {school_dist:.2f} км"
```