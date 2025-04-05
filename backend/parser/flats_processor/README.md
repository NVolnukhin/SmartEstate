# SmartEstate FlatsProcessor
## Описание Модуля
Этот модуль предназначен для обработки данных о зданиях и квартирах из CSV-файла, их геокодирования и сохранения в базу данных PostgreSQL.

## Структура
```
actualpriceparser/
├── calculator.py                   # Основной модуль расчета времени пути
├── location.py                     # Модель данных для хранения координат
├── main.py                         # Точка входа для запуска обработки
├── .env                            # Файл конфигурации
├── README.md                       # Документация
└── storage
      ├── buildings_db_storage.py   # Модуль взаимодействя с БД зданий 
      └── flats_db_storage.py       # Модуль взаимодействя с БД квартир
```
## Требования
- Python 3.8+
- Установленные зависимости из requirements.txt 
- API-ключ Яндекс.Карт (добавляется в .env файл как YANDEX_API_KEY)
- Доступ к PostgreSQL с соответствующими правами



##  Установка
Клонируйте репозиторий:
```commandline
git clone https://git.iu7.bmstu.ru/sgn3-prog/sgn3-prog-it-2025/ghostbusters.git
cd backend/parser/flatsprocessor
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
YANDEX_API_KEY=your_yandex_api_key
```
2. Убедитесь, что CSV-файл с данными (flats_parse_result.csv) находится в директории выше корневой (../)


## Структура БД:

Модуль создает две таблицы:

### Buildings
- BuildingId - идентификатор здания 
- DeveloperId - идентификатор застройщика (внешний ключ)
- ConstructionStatus - статус строительства 
- FloorCount - количество этажей 
- Address - адрес здания 
- InfrastructureInfoId - идентификатор информации об инфраструктуре (не используется)
- Latitude - широта 
- Longitude - долгота

### Flats
- FlatId - идентификатор квартиры 
- Square - площадь 
- Roominess - количество комнат 
- Floor - этаж 
- CianLink - ссылка на CIAN 
- BuildingId - идентификатор здания (внешний ключ)
- FinishType - тип отделки


## Использование
Запуск основного скрипта:

``` python main.py```

## Особенности работы
- При каждом запуске данные в таблицах полностью очищаются (TRUNCATE) перед вставкой новых 
- Для геокодирования используется API Яндекс.Карт 
- Адреса зданий формируются в формате: "г. {город}, {улица}, {дом}"

