# SmartEstate Metro Parser
## Сбор актуальных данных о метро Москвы

### Подробная информация
Metroparser - это библиотека Python 3 (версии 3.8 и выше) для парсинга страницы [Wikipedia](https://ru.wikipedia.org/wiki/%D0%A1%D0%BF%D0%B8%D1%81%D0%BE%D0%BA_%D1%81%D1%82%D0%B0%D0%BD%D1%86%D0%B8%D0%B9_%D0%9C%D0%BE%D1%81%D0%BA%D0%BE%D0%B2%D1%81%D0%BA%D0%BE%D0%B3%D0%BE_%D0%BC%D0%B5%D1%82%D1%80%D0%BE%D0%BF%D0%BE%D0%BB%D0%B8%D1%82%D0%B5%D0%BD%D0%B0) c актуальным списком станций Московского Метрополитена. Данная библиотека предоставляет возможность получения актуального списка станций метро в Москве со следующими данными: 
- Название
- Координаты
  - Широта
  - Долгота

А так же позволяет сохранятть данные в файл и в базу данных 

### Установка
```pip install metroparser```

### Настройка Парсера 
```
from metroparser import parse_wiki_metro_stations

stations = parse_wiki_metro_stations()
if not stations:
    print("Не удалось получить данные станций")
    return

print(f"\nНайдено станций: {len(stations)}\n")

```

### Настройка Сохранения в файл 
```
from storage.file_storage import save_to_csv

save_to_csv(stations)
```

### Настройка Сохранения в БД 
```
from storage.db_storage import save_to_postgresql

db_params = {
    'dbname': 'your_db_name',       #for example - smartestatedb
    'user': 'your_username',        #for example - postgres
    'password': 'your_password',    #for example - pass
    'host': 'your_host',            #for example - localhost
    'port': 'ypur_port'             #for example - 5432
}
save_to_postgresql(stations, db_params)
```
### Запуск 

```commandline
/opt/anaconda3/bin/python3 path_to_directory/main.py
```