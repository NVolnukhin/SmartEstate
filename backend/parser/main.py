import cianparser.cianparser as cp

moscow_parser = cp.CianParser(location="Москва")
data = moscow_parser.get_flats(deal_type="sale",
                               rooms="all",
                               with_saving_csv=True,
                               # with_extra_data=True,
                               additional_settings={
                                    "object_type": "new",
                                    "start_page":1,
                                    # "end_page": 1,
                                    "district": {"Некрасовка", "Ново-Переделкино", "Басманный"}
                               })