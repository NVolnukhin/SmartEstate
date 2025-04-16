# SmartEstate ActualPriceParser
## Описание Модуля
Этот модуль предназначен для автоматического сбора текущих цен квартир с сайта CIAN и сохранения их в базу данных PostgreSQL с историей изменений.

## Структура
```
actualpriceparser/
├── get_actual_price.py  # Модуль парсинга цен с CIAN
├── batcher.py          # Обработчик пакетов данных
├── main.py             # Основной скрипт запуска
├── .env                # Файл с переменными окружения
└── README.md           # Документация
```
## Требования
- Python 3.8+
- PostgreSQL
- Установленные зависимости из requirements.txt

##  Установка
1. Клонируйте репозиторий:
    ```commandline
    git clone https://git.iu7.bmstu.ru/sgn3-prog/sgn3-prog-it-2025/ghostbusters.git
    cd backend/parser/actualpriceparser
    ```

2. Установите зависимости:

    ```pip install -r requirements.txt```

3. Создайте файл .env в корне проекта с параметрами подключения к БД:
    
    ```commandline
    DB_NAME=your_database
    DB_USER=your_username
    DB_PASSWORD=your_password
    DB_HOST=localhost
    DB_PORT=5432
    ```

## Использование
Запуск основного скрипта:

``` python main.py```

Скрипт будет:

1. Получать данные о квартирах из таблицы Flats блоками по 10 записей

2. Парсить текущие цены с CIAN по указанным ссылкам

3. Сохранять цены с меткой времени в таблицу PriceHistories

## Структура базы данных
Необходимые таблицы:

```sql 
CREATE TABLE "Flats" (
    "FlatId" SERIAL PRIMARY KEY,
    "CianLink" TEXT NOT NULL,
    -- другие поля
);

CREATE TABLE "PriceHistories" (
    "PriceHistoryId" SERIAL PRIMARY KEY,
    "FlatId" INTEGER REFERENCES "Flats"("FlatId"),
    "Price" NUMERIC NOT NULL,
    "ChangeDate" TIMESTAMP WITH TIME ZONE DEFAULT (NOW() AT TIME ZONE 'UTC')
);
```

## Настройка
- Интервал между запросами: измените time.sleep(5) в main.py
- Размер пакета: измените LIMIT 10 в batcher.py
- User-Agent: настройте в get_actual_price.py

## Логирование
Скрипт выводит в консоль:

- Успешные добавления в БД

- Ошибки парсинга

- Общий прогресс обработки

## Ограничения
- Скрипт может перестать работать при изменении структуры сайта CIAN

- Возможна блокировка IP при слишком частых запросах

- Не обрабатывает случаи капчи

