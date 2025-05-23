# SmartEstate GeoJSON (OSM) to CSV Parser
## Конвертация GeoJSON (OSM) в CSV

### Описание
GeoJSON Parser - это Python скрипт (версии 3.8 и выше) для конвертации файлов в формате GeoJSON с сервиса OSM в удобный CSV формат с автоматическим определением полей. Парсер извлекает следующие данные:
- Уникальный ID (генерируется автоматически)
- Название объекта (автоматическое определение поля)
- Географические координаты
  - Широта 
  - Долгота 

### Установка
```bash
pip install geojsontocsvparser
```
### Использование
#### Базовый запуск
```bash
from geojsontocsvparser import main

main()
```
#### После запуска:

1) Выберите входной GeoJSON файл через диалоговое окно
2) Введите имя для выходного CSV файла (без расширения)
3) Получите результат в папке secondary_data

#### Пример работы с парсером
```bash
from geojsontocsvparser import (
    load_geojson,
    process_geojson_features,
    write_to_csv
)

data = load_geojson("input.geojson")
if not data:
    raise ValueError("Не удалось загрузить файл")

features = data.get('features', [])
csv_data, name_field = process_geojson_features(features)

write_to_csv(csv_data, "output.csv")
```
#### Формат выходных данных
Результат сохраняется в CSV файл со следующими колонками:

- ID - уникальный идентификатор
- name - название объекта
- latitude - широта
- longitude - долгота

#### Пример выходных данных:

```
ID,name,latitude,longitude
1,Центральный парк,55.7338,37.6069
2,Музей искусств,55.7412,37.6203
3,Главный вокзал,55.7525,37.6321
...
```
### Логирование работы
Программа выводит в консоль информацию о ходе выполнения:

- Путь к выбранному файлу
- Количество обработанных объектов
- Путь к сохраненному файлу
- Ошибки (если возникают)