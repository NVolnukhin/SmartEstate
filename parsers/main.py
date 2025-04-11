import cianparser.cianparser as cp

msk_parser = cp.CianParser(location="Москва")
data = msk_parser.get_flats(deal_type="sale",
                               rooms="all",
                               with_saving_csv=True,
                               with_extra_data=True,
                               additional_settings={
                                    "object_type": "new",
                                    "author_type": "застройщик",
                                    "new_moscow": "without",
                                    "start_page":1,
                                    "end_page": 54,
                               })