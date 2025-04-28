# SmartEstate GraphHooper
## Описание Модуля
Модуль предназначен для расчета времени пешего пути от зданий до ближайших объектов инфраструктуры (детские сады, школы, метро, аптеки, магазины) с использованием API GraphHopper и сохранения результатов в базу данных PostgreSQL.

## Структура
```
actualpriceparser/
├── calculator.py       # Основной модуль расчета времени пути
├── location.py         # Модель данных для хранения координат
├── main.py             # Точка входа для запуска обработки
├── .env                # Файл конфигурации
└── README.md           # Документация
```
## Требования
- Python 3.8+
- PostgreSQL 12+
- Учетные записи GraphHopper API (до 4 ключей)
- Установленные зависимости из requirements.txt

##  Установка
Клонируйте репозиторий:
```commandline
git clone https://git.iu7.bmstu.ru/sgn3-prog/sgn3-prog-it-2025/ghostbusters.git
cd backend/parser/graphhooper
```

Установите зависимости:

```pip install -r requirements.txt```

Создайте файл .env в корне проекта с параметрами подключения к БД:

```commandline
DB_NAME=your_database
DB_USER=your_username
DB_PASSWORD=your_password
DB_HOST=localhost
DB_PORT=5432
```

## Настройка
1. Создайте файл .env в корне проекта:


```
DB_NAME=your_database
DB_USER=your_username
DB_PASSWORD=your_password
DB_HOST=localhost
DB_PORT=5432
GRAPH_HOOPER_API_KEY_1=your_api_key_1
GRAPH_HOOPER_API_KEY_2=your_api_key_2
GRAPH_HOOPER_API_KEY_3=your_api_key_3
GRAPH_HOOPER_API_KEY_4=your_api_key_4
```
2. Структура БД должна содержать следующие таблицы:
```sql
-- Здания
CREATE TABLE "Buildings" (
    "BuildingId" SERIAL PRIMARY KEY,
    "Latitude" FLOAT NOT NULL,
    "Longitude" FLOAT NOT NULL
);

-- Инфраструктурная информация
CREATE TABLE "InfrastructureInfos" (
    "BuildingId" INTEGER PRIMARY KEY REFERENCES "Buildings"("BuildingId"),
    "NearestKindergartenId" INTEGER,
    "NearestSchoolId" INTEGER,
    "NearestMetroId" INTEGER,
    "NearestPharmacyId" INTEGER,
    "NearestShopId" INTEGER,
    "MinutesToKindergarten" INTEGER,
    "MinutesToSchool" INTEGER,
    "MinutesToMetro" INTEGER,
    "MinutesToPharmacy" INTEGER,
    "MinutesToShop" INTEGER
);

-- Таблицы объектов инфраструктуры (пример для детских садов)
CREATE TABLE "Kindergarten" (
    "Id" SERIAL PRIMARY KEY,
    "Latitude" FLOAT NOT NULL,
    "Longitude" FLOAT NOT NULL
);
```

## Использование
Запуск основного скрипта:

``` python main.py```

Параметры обработки:

1. batch_size (по умолчанию 131) - количество зданий, обрабатываемых за один цикл

2. Автоматическая ротация между 4 API-ключами GraphHopper

3. Пауза 1 секунда между запросами к API

## Особенности реализации
1. Класс Location: Хранит координаты объектов (широта/долгота)
2. Методы WalkingTimeCalculator:
   - calculate_walking_time() - расчет времени пути через GraphHopper API
   - update_walking_times() - обновление данных в БД
   - process_buildings() - основной цикл обработки зданий

3. Обработка ошибок:
    - Логирование ошибок подключения к БД
    - Логирование ошибок API GraphHopper
    - Автоматический откат транзакций при ошибках

4. Оптимизации:
   - Пакетная обработка зданий
   - Минимизация количества запросов к API
   - Использование соединения с БД в рамках одной сессии

## Рекомендации по использованию
1. Для больших объемов данных рекомендуется:

   - Увеличить интервалы между запросами 
   - Использовать больше API-ключей
   - Запускать в ночное время

2. Для мониторинга:

   - Логировать прогресс обработки
   - Сохранять статистику выполнения

3. При изменении структуры БД:

   - Обновить SQL-запросы в calculator.py

   - Проверить соответствие полей в методе process_buildings()


## Ограничения
1. Ограничения API GraphHopper:

   - Лимиты на количество запросов 
   - Требование API-ключей 
   - Задержки между запросами

2. Зависимость от:

   - Доступности API GraphHopper 
   - Актуальности данных в БД 
   - Корректности координат объектов



