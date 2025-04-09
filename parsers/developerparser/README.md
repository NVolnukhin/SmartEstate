# SmartEstate DeveloperParser
## Описание Модуля
Этот модуль предназначен для сбора информации о застройщиках с сайта CIAN.ru и сохранения данных в CSV-файл и базу данных PostgreSQL

## Структура
```
developerparser/
├── developersparser.py             # Содержит функцию для парсинга данных
├── main.py                         # Точка входа для запуска обработки
├── .env                            # Файл конфигурации
├── README.md                       # Документация
└── storage
      ├── db_storage.py             # Функции для работы с базой данных 
      └── file_storage.py           # Функции для работы с файлами
```
## Требования
- Python 3.8+
- Установленные зависимости
  - requests
  - beautifulsoup4
  - psycopg2
  - python-dotenv
- Доступ к PostgreSQL с соответствующими правами



##  Установка
Клонируйте репозиторий:
```commandline
git clone https://git.iu7.bmstu.ru/sgn3-prog/sgn3-prog-it-2025/ghostbusters.git
cd backend/parser/flatsprocessor
```

Установите зависимости:

```pip install -r requirements.txt```

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

## Структура БД:

Модуль создает две таблицы:

### Developers
- Id - идентификатор застройщика (автоинкремент)
- Name - название компании-застройщика
- Website - ссылка на профиль на CIAN
- BuildingsCount - количество построенных объектов (в текстовом формате)

### Формат CSV-файла
Результирующий файл содержит колонки:

- num - порядковый номер 
- name - название застройщика
- website - ссылка на профиль


## Использование
Запуск основного скрипта:

``` python main.py```

## Особенности работы
- Парсер обходит 95 страниц с застройщиками на CIAN (можно изменить параметром pages)
- Между запросами к страницам стоит задержка 2 секунды (time.sleep)
- При каждом запуске данные в таблице Developers полностью очищаются перед вставкой новых
- Данные сохраняются как в базу данных, так и в CSV-файл


