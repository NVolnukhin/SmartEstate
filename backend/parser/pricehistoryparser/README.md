# SmartEstate Price History Parser
Этот модуль предназначен для сбора истории изменения цен на квартиры с сайта CIAN и сохранения данных в PostgreSQL базу данных.

## Структура проекта
```
├── README.md
├── main.py                             # Главный скрипт для запуска сбора данных
├── price_history_parser.py             # Парсер истории цен с CIAN
└── storage/
    ├── get_db_connection.py            # Утилита для подключения к БД
    ├── get_links_from_db.py            # Получение ссылок на объявления из БД
    └── save_price_history_to_db.py     # Сохранение истории цен в БД
```

## Функциональность
- Парсинг истории цен с страницы объявления на CIAN

- Сохранение данных в базу данных PostgreSQL

- Поддержка обработки множества объявлений

- Защита от дублирования данных (ON CONFLICT)

## Требования
- Python 3.8+
- Установленные зависимости из requirements.txt
- Доступ к PostgreSQL базе данных
- Файл .env с параметрами подключения к БД

## Установка
1. Клонируйте репозиторий:
    ```commandline
    git clone https://git.iu7.bmstu.ru/sgn3-prog/sgn3-prog-it-2025/ghostbusters.git
    cd backend/parser/pricehistoryparser
    ```
2. Установите зависимости
```pip install -r requirements.txt```
3. Создайте файл .env в корне проекта с параметрами подключения к БД:
    ```
    DB_NAME=your_db_name
    DB_USER=your_db_user
    DB_PASSWORD=your_db_password
    DB_HOST=your_db_host
    DB_PORT=your_db_port
    ```
## Запуск
```python main.py```

## Настройка
Модуль можно настроить, изменив:

- Задержку между запросами (в main.py)
- Параметры парсинга (в price_history_parser.py)
- Структуру запросов к БД (в файлах в папке storage)

## База данных
Модуль предполагает наличие следующих таблиц в БД:

- Flats с колонками FlatId и CianLink
- PriceHistories с колонками FlatId, Price, ChangeDate

```sql
CREATE TABLE "Flats" (
    "FlatId" SERIAL PRIMARY KEY,
    "CianLink" TEXT NOT NULL
);

CREATE TABLE "PriceHistories" (
    "Id" SERIAL PRIMARY KEY,
    "FlatId" INTEGER REFERENCES "Flats"("FlatId"),
    "Price" INTEGER NOT NULL,
    "ChangeDate" DATE NOT NULL,
    UNIQUE ("FlatId", "ChangeDate")
);
```

## Ограничения
1. Модуль зависит от структуры HTML страницы CIAN и может перестать работать при её изменении

2. Для корректного парсинга дат требуется русская локаль на сервере

