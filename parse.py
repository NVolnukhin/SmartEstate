import  smart_estate_parser

moscow_parser = smart_estate_parser.SmartEstateParser(location="Москва")
data = moscow_parser.get_flats(rooms="all", deal_type="sale", district=78, with_saving_csv=False)

print(data[0])

#Метро Некрасовка 373
#Район Некрасовка 78
