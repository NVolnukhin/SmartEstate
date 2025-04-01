# SmartEstate Parser
### Сбор данных с сайта объявлений об аренде и продаже недвижимости Циан
Cianparser - это библиотека Python 3 (версии 3.8 и выше) для парсинга сайта Циан. С его помощью можно получить достаточно подробные и структурированные данные по краткосрочной и долгосрочной аренде, продаже квартир, домов, танхаусов итд.

### Установка
```pip install cianparser```

### Использование
```
import cianparser

moscow_parser = cianparser.CianParser(location="Москва")
data = moscow_parser.get_flats(deal_type="sale", rooms=(1, 2), with_saving_csv=True, additional_settings={"start_page":1, "end_page":2})

print(data[0])
```

```
                              Preparing to collect information from pages..
The absolute path to the file: 
 /Users/macbook/some_project/cianparser/cian_flat_sale_1_2_moskva_12_Jan_2024_21_48_43_100892.csv 

The page from which the collection of information begins: 
 https://cian.ru/cat.php?engine_version=2&p=1&with_neighbors=0&region=1&deal_type=sale&offer_type=flat&room1=1&room2=1

Collecting information from pages with list of offers
 1 | 1 page with list: [=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>] 100% | Count of all parsed: 28. Progress ratio: 50 %. Average price: 45 547 801 rub
 2 | 2 page with list: [=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>=>] 100% | Count of all parsed: 56. Progress ratio: 100 %. Average price: 54 040 102 rub

The collection of information from the pages with list of offers is completed
Total number of parsed offers: 56.
{
    "author": "MR Group",
    "author_type": "developer",
    "url": "https://www.cian.ru/sale/flat/292125772/",
    "location": "Москва",
    "deal_type": "sale",
    "accommodation_type": "flat",
    "floor": 20,
    "floors_count": 37,
    "rooms_count": 1,
    "total_meters": 39.6,
    "price": 28623910,
    "district": "Беговой",
    "street": "Ленинградский проспект",
    "house_number": "вл8",
    "underground": "Белорусская",
    "residential_complex": "Slava"
}
```

### Инициализация
Параметры, используемые при инициализации парсера через функциою CianParser:

- location - локация объявления, к примеру, Москва (для просмотра доступных мест используйте cianparser.list_locations())
- proxies - прокси (см раздел Cloudflare, CloudScraper, Proxy), по умолчанию None

## Метод get_flats
Данный метод принимает следующий аргументы:

- deal_type - тип объявления, к примеру, долгосрочная аренда, продажа ("rent_long", "sale")
- rooms - количество комнат, к примеру, 1, (1,3, "studio"), "studio, "all"; по умолчанию любое ("all")
- with_saving_csv - необходимо ли сохранение собираемых данных (в реальном времени в процессе сбора данных) или нет, по умолчанию False
- with_extra_data - необходимо ли сбор дополнительных данных, но с кратным продолжительности по времени (см. ниже в Примечании), по умолчанию False
- additional_settings - дополнительные настройки поиска (см. ниже в Дополнительные настройки поиска), по умолчанию None

Пример:

```
import cianparser

moscow_parser = cianparser.CianParser(location="Москва")
data = moscow_parser.get_flats(deal_type="rent_long", rooms=(1, 2), additional_settings={"start_page":1, "end_page":1})
```

В проекте предусмотрен функционал корректного завершения в случае окончания страниц. По данному моменту, следует изучить раздел Ограничения


## Дополнительные настройки поиска

Пример:
```
additional_settings = {
    "start_page":1,
    "end_page": 10,
    "is_by_homeowner": True,
    "min_price": 1000000,
    "max_price": 10000000,
    "min_balconies": 1,
    "have_loggia": True,
    "min_house_year": 1990,
    "max_house_year": 2023,
    "min_floor": 3,
    "max_floor": 4,
    "min_total_floor": 5,
    "max_total_floor": 10,
    "house_material_type": 1,
    "metro": "Московский",
    "metro_station": "ВДНХ",
    "metro_foot_minute": 45,
    "flat_share": 2,
    "only_flat": True,
    "only_apartment": True,
    "sort_by": "price_from_min_to_max",
}
```
- object_type - тип жилья ("new" - вторичка, "secondary" - новостройка)
- start_page - страница, с которого начинается сбор данных
- end_page - страница, с которого заканчивается сбор данных
- is_by_homeowner - объявления, созданных только собственниками
- min_price - цена от (в рублях)
- max_price - цена до (в рублях)
- min_balconies - минимальное количество балконов
- have_loggia - наличие лоджи
- min_house_year - год постройки дома от
- max_house_year - год постройки дома до
- min_floor - этаж от
- max_floor - этаж до
- min_total_floor - этажей в доме от
- max_total_floor - этажей в доме до
- house_material_type - тип дома (см ниже возможные значения)
- metro - название метрополитена (см ниже возможные значения)
- metro_station - станция метро (доступно при заданом metro)
- metro_foot_minute - сколько минут до метро пешком
- flat_share - с долями или без (1 - только доли, 2 - без долей)
- only_flat - без апартаментов
- only_apartment - только апартаменты
- sort_by - сортировка объявлений (см ниже возможные значения)

## Возможные значения поля house_material_type
- 1 - киричный
- 2 - монолитный
- 3 - панельный
- 4 - блочный
- 5 - деревянный
- 6 - сталинский
- 7 - щитовой
- 8 - кирпично-монолитный

#### Возможные значения полей metro и metro_station
Соответствуют ключам и значениям словаря, получаемого вызовом функции cianparser.list_metro_stations()

## Возможные значения поля sort_by
- "price_from_min_to_max" - сортировка по цене (сначала дешевле)
- "price_from_max_to_min" - сортировка по цене (сначала дороже)
- "total_meters_from_max_to_min" - сортировка по общей площади (сначала больше)
- "creation_data_from_newer_to_older" - сортировка по дате добавления (сначала новые)
- "creation_data_from_older_to_newer" - сортировка по дате добавления (сначала старые)

## Признаки, получаемые в ходе сбора данных с предложений по долгосрочной аренде недвижимости
- district - район
- underground - метро
- street - улица
- house_number - номер дома
- floor - этаж
- floors_count - общее количество этажей
- total_meters - общая площадь
- living_meters - жилая площади
- kitchen_meters - площадь кухни
- rooms_count - количество комнат
- year_construction - год постройки здания
- house_material_type - тип дома (киричный/монолитный/панельный итд)
- heating_type - тип отопления
- price_per_month - стоимость в месяц
- commissions - комиссия, взымаемая при заселении
- author - автор объявления
- author_type - тип автора
- phone - номер телефона в объявлении
- url - ссылка на объявление

## Возможные значения поля author_type:

- real_estate_agent - агентство недвижимости
- homeowner - собственник
- realtor - риелтор
- official_representative - ук оф.представитель
- representative_developer - представитель застройщика
- developer - застройщик
- unknown - без указанного типа

### Признаки, получаемые в ходе сбора данных с предложений по продаже недвижимости
## Признаки аналогичны вышеописанным, кроме отсутствия полей price_per_month и commissions.

При этом появляются новые:

- price - стоимость недвижимости
- residential_complex - название жилого комплекса
- object_type - тип жилья (вторичка/новостройка)
- finish_type - отделка
- Признаки, получаемые в ходе сбора данных по новостройкам
- name - наименование ЖК
- url - ссылка на страницу
- full_location_address - полный адрес расположения ЖК
- year_of_construction - год сдачи
- house_material_type - тип дома (см выше возможные значения)
- finish_type - отделка
- ceiling_height - высота потолка
- class - класс жилья
- parking_type - тип парковки
- floors_from - этажность (от)
- floors_to - этажность (до)
- builder - застройщик

## Сохранение данных
Имеется возможность сохранения собираемых данных в режиме реального времени. Для этого необходимо подставить в аргументе with_saving_csv значение True.

## Пример получаемого файла при вызове метода get_flats с with_extra_data = True:
```cian_flat_sale_1_1_moskva_12_Jan_2024_22_29_48_117413.csv```
```author	author_type	url	location	deal_type	accommodation_type	floor	floors_count	rooms_count	total_meters	price_per_m2	price	year_of_construction	object_type	house_material_type	heating_type	finish_type	living_meters	kitchen_meters	phone	district	street	house_number	underground	residential_complex
White and Broughton	real_estate_agent	https://www.cian.ru/sale/flat/290499455/	Москва	sale	flat	3	40	1	45.5	709890	32300000	2021	Вторичка	Монолитный	Центральное	-1	19.0	6.0	+79646331510	Хорошевский	Ленинградский проспект	37/4	Динамо	Прайм Парк
ФСК	developer	https://www.cian.ru/sale/flat/288376323/	Москва	sale	flat	24	47	2	46.0	528900	24329400	2024	Новостройка	Монолитно-кирпичный	-1	Без отделки, предчистовая, чистовая	18.0	15.0	+74951387154	Обручевский	Академика Волгина	2С1	Калужская	Архитектор
White and Broughton	real_estate_agent	https://www.cian.ru/sale/flat/292416804/	Москва	sale	flat	2	41	2	60.0	783333	47000000	2021	Вторичка	-1	Центральное	-1	43.0	5.0	+79646331510	Хорошевский	Ленинградский проспект	37/5	Динамо	Прайм Парк
```

Пример получаемого файла при вызове метода get_newobjects:
```
cian_newobject_13_Jan_2024_01_27_32_734734.csv
```
```
name	location	accommodation_type	url	full_location_address	year_of_construction	house_material_type	finish_type	ceiling_height	class	parking_type	floors_from	floors_to	builder
ЖК «SYMPHONY 34 (Симфони 34)»	Москва	newobject	https://zhk-symphony-34-i.cian.ru	Москва, САО, Савеловский, 2-я Хуторская ул., 34	2025	Монолитный	Предчистовая, чистовая	3,0 м	Премиум	Подземная, гостевая	36	54	Застройщик MR Group
ЖК «Коллекция клубных особняков Ильинка 3/8»	Москва	newobject	https://zhk-kollekciya-klubnyh-osobnyakov-ilinka-38-i.cian.ru	Москва, ЦАО, Тверской, ул. Ильинка	2024	Монолитно-кирпичный, монолитный	Без отделки	от 3,35 м до 6,0 м	Премиум	Подземная, гостевая	3	5	Застройщик Sminex-Интеко
ЖК «Victory Park Residences (Виктори Парк Резиденсез)»	Москва	newobject	https://zhk-victory-park-residences-i.cian.ru	Москва, ЗАО, Дорогомилово, ул. Братьев Фонченко	2024	Монолитный	Чистовая	—	Премиум	Подземная	10	11	Застройщик ANT Development
```

## Cloudflare, CloudScraper, Proxy
Для обхода блокировки в проекте задействован CloudScraper (библиотека cloudscraper), который позволяет успешно обходить защиту Cloudflare.

Вместе с тем, это не гарантирует отсутствие возможности появления у некоторых пользователей теста CAPTCHA при долговременном непрерывном использовании.

### Proxy
Поэтому была предоставлена возможность проставлять прокси, используя аргумент proxies (список прокси протокола HTTPS)

Пример:

```proxies = [
    '117.250.3.58:8080', 
    '115.96.208.124:8080',
    '152.67.0.109:80', 
    '45.87.68.2:15321', 
    '68.178.170.59:80', 
    '20.235.104.105:3729', 
    '195.201.34.206:80',
]
```

В процессе запуска утилита проходится по всем из них, пытаясь определить подходящий, то есть тот, который может, во первых, делать запросы, во вторых, не иметь тест CAPTCHA

Пример лога, в котором представлено все три возможных кейса

```
The process of checking the proxies... Search an available one among them...
 1 | proxy 46.47.197.210:3128: unavailable.. trying another
 2 | proxy 213.184.153.66:8080: there is captcha.. trying another
 3 | proxy 95.66.138.21:8880: available.. stop searching
```

## Ограничения
Сайт выдает списки с объявлениями лишь до 54 странцы включительно. Это примерно 28 * 54 = 1512 объявлений. Поэтому, если имеется желание собрать как можно больше данных, то следует использовать более конкретные запросы (по количеству комнат).

К примеру, вместо того, чтобы при использовании указывать rooms=(1, 2), стоит два раза отдельно собирать данные с параметрами rooms=1 и rooms=2 соответственно.

Таким образом, максимальная разница может составить 1 к 6 (студия, 1, 2, 3, 4, 5 комнатные квартиры), то есть 1512 к 9072.

## Примечание
В некоторых объявлениях отсутсвуют данные по некоторым признакам (год постройки, жилые кв метры, кв метры кухни итп). В этом случае проставляется значение -1 либо пустая строка для числового и строкового типа поля соответственно.

Для отсутствия блокировки по IP в данном проекте задана пауза (в размере 4-5 секунд) после сбора информации с каждой отдельной взятой страницы.

Имеется флаг with_extra_data, при помощи которого можно дополнительно собирать некоторые данные, но при этом существенно (в 5-10 раз) замедляется процесс по времени, из-за необходимости заходить на каждую страницу с предложением. Соответствующие данные: площадь кухни, год постройки здания, тип дома, тип отделки, тип отопления, тип жилья и номер телефона.
