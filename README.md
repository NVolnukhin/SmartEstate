# SmartEstate - Система сравнения квартир по районам Москвы

## 📖 О проекте

SmartEstate — это интеллектуальная информационная система, разработанная для удобного анализа и сравнения квартир в новостройках по г. Москва. Проект ориентирован на потенциальных покупателей и застройщиков, предоставляя детальную аналитику и удобные инструменты для выбора жилья.

### 🔹 Основные возможности:

- Сравнение квартир по ключевым параметрам (цена, площадь, количество комнат, застройщик и др.).

- Анализ динамики цен за время строительства.

- Оценка расположения объектов относительно инфраструктуры (школы, детские сады, метро, магазины и т. д.).

- Актуальные данные с официальных сайтов застройщиков.
## 🛠️ Технологический стек

### 🎯 Бэкенд

- **Язык**: C#

- **Фреймворк**: ASP.NET Core

### 🔍 Парсинг и нормализация данных

- **Язык**: Python

- **Библиотеки**: 
  - BeautifulSoup4
  - Scrapy
  - Requests
  - PsyCopg2
  - Selenium
  - Python-Dotenv
  - CloudScrapper
  - Transliterate
  - Pandas
  - Numpy

### 💻 Фронтенд

- **Язык**: JavaScript (ES6+)  
  _(TypeScript не используется - чистый ванильный JS)_

- **Фреймворки**: Нет (чистый JavaScript)  
  _(Нативный DOM API)_

- **Стилизация**: CSS

- **Управление состоянием**: Нативный JavaScript


### 🎯 Дополнительные сервисы

PostgreSQL (используется расширение postgis)

Картографические сервисы: GraphHooper, Yandex Maps

## 📊 Источники данных

Информация о квартирах собирается с официальных сайтов застройщиков:

- #### 🏗️ [Циан](https://www.cian.ru/) — база проверенных объявлений о продаже и аренде жилой, загородной и коммерческой недвижимости. Онлайн-сервис №1 в России в категории «Недвижимость», по данным Similarweb на сентябрь 2023 г. 

## 📌 Аналоги

|        Платформа        |  Достоинства |   Недостатки   |
|:-----------------------:|:------------:|:--------------:|
|        [Циан](https://www.cian.ru/)         |   Широкая база, удобный поиск   |    Нет детального сравнения объектов    |
| [Яндекс.Недвижимость](https://realty.yandex.ru/) |  Интеграция с картами, охват регионов   |   Ограниченный функционал для анализа   |

## 🎓 Основания для разработки

Проект разрабатывается в рамках дисциплин “Интернет-технологии” и “Программная инженерия” в МГТУ им. Н.Э. Баумана.

***

## 🚀 Запуск проекта

### 🔹 Требования:

-  Node.js (для фронтенда)

- .NET SDK (для бэкенда)

- Python 3.x (для парсинга)

### 🔹 Инструкции по запуску:

#### Запуск бэкенда:

1. Установить .NET SDK и PostgreSQL.
2. Клонировать репозиторий:
```commandLine
git clone https://git.iu7.bmstu.ru/sgn3-prog/sgn3-prog-it-2025/ghostbusters.git
```
3. Перейти в директорию проекта и установить зависимости:
```commandline
 cd backend
dotnet restore
```
4. Установить зависимости Python:
```commandline
pip install -r requirements.txt
```
5. Настроить подключение к базе данных в `appsettings.json` (для C#) и `.env` (для Python).
6. Запустить сервер Python:
```commandline
uvicorn main:app --reload
```
7. Запустить сервер C#:
```commandline
dotnet run
```


#### Запуск фронтенда:

1. Установить зависимости не требуется (проект работает без Node.js/npm)
2. Просто откройте index.html в браузере:
   - Либо двойным кликом по файлу
   - Либо через Live Server в VS Code (если используется)



#### Запуск парсинга данных:
```commandline
cd parsers
pip install -r requirements.txt
python main.py
```

## Docker-развертывание проекта

Проект использует Docker Compose для запуска всех необходимых сервисов:

### Состав сервисов:
- **PostgreSQL 14** - основная база данных
  - Порт: `DB_PORT`
  - Данные сохраняются в volume `pg_data`
- **pgAdmin 4** - веб-интерфейс для управления PostgreSQL
  - Доступ: `http://localhost:5050`
  - Логин: `DB_USER_LOGIN`
  - Пароль: `DB_USER_PASSWORD`
- **Backend** - ASP.NET Core приложение
  - Порт: `BACKEND_PORT`
  - Автоматически применяет миграции при запуске
- **Frontend** - Веб-интерфейс
  - Порт: `FRONTEND_PORT`
  - Доступ: `http://localhost`

### Инструкция по запуску:

1. Установить:
   - Docker
   - Docker Compose

2. Собрать и запустить контейнеры:
```bash
docker-compose up -d --build
```
После запуска сервисы будут доступны:

**Фронтенд**: `http://localhost`

**Бэкенд**: `http://localhost:BACKEND_PORT`

**pgAdmin**: `http://localhost:5050`

**База данных**: `localhost:DB_PORT`

#### Для остановки:

```bash
docker-compose down
```

## Настройка окружения
### .env файл
Для работы приложения необходимо создать файл `.env` в корне проекта со следующими переменными:

### Обязательные переменные:
```env
# Учетные данные для базы данных
DB_USER
DB_PASSWORD
DB_NAME
DB_HOST
DB_PORT

# Учетные данные для админки БД
DB_USER_LOGIN
DB_USER_PASSWORD

# Названия таблиц в БД
DB_KINDERGARTEN_TABLE_NAME="Kindergarten"
DB_SCHOOL_TABLE_NAME="School"
DB_SHOP_TABLE_NAME="Shop"
DB_PHARMACY_TABLE_NAME="Pharmacy"
DB_METRO_TABLE_NAME="Metro"
DB_BUILDINGS_TABLE_NAME="Buildings"

# Настройки окружения
ASPNETCORE_ENVIRONMENT
BACKEND_PORT
FRONTEND_PORT

API-ключи (необходимы для работы геосервисов):
# Ключ API Яндекс.Карт
YANDEX_API_KEY="ваш_ключ"

# Ключи API GraphHopper (можно указать до 7 ключей для ротации)
GRAPH_HOOPER_API_KEY_1="ключ_1"
GRAPH_HOOPER_API_KEY_2="ключ_2"
GRAPH_HOOPER_API_KEY_3="ключ_3"
GRAPH_HOOPER_API_KEY_4="ключ_4"
GRAPH_HOOPER_API_KEY_5="ключ_5"
GRAPH_HOOPER_API_KEY_6="ключ_6"
GRAPH_HOOPER_API_KEY_7="ключ_7"
```

### config.js 
Так же для работы приложения необходимо создать файл `config.js` в директории `frontend/js/` со следующего содержания:
```
export const config = {
    api: {
      baseUrl: 'your_base_url/api',
      ymaps_api_key: 'API ключ Яндекс Карт'
    },
    hash: {
      salt: 'Соль для хэширования пароля на стороне клиента'
    }
  };
  
  export const date = 'Дата актуальности данных'
```

## 📩 Контакты
Студент СГН3-42Б

- Волнухин Н. Д. 

