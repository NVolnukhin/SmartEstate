import hashlib
import math
import csv
import os
import sys
from pathlib import Path

from .constants import SPECIFIC_FIELDS_FOR_RENT_LONG, SPECIFIC_FIELDS_FOR_RENT_SHORT, SPECIFIC_FIELDS_FOR_SALE


class BaseListPageParser:
    def __init__(self,
                 session,
                 accommodation_type: str, deal_type: str, rent_period_type, location_name: str,
                 with_saving_csv=False, with_extra_data=False,
                 object_type=None, additional_settings=None):
        self.accommodation_type = accommodation_type
        self.session = session
        self.deal_type = deal_type
        self.rent_period_type = rent_period_type
        self.location_name = location_name
        self.with_saving_csv = with_saving_csv
        self.with_extra_data = with_extra_data
        self.additional_settings = additional_settings
        self.object_type = object_type

        self.result = []
        self.result_set = set()
        self.average_price = 0
        self.count_parsed_offers = 0
        self.start_page = 1 if (additional_settings is None or "start_page" not in additional_settings.keys()) else additional_settings["start_page"]
        self.end_page = 100 if (additional_settings is None or "end_page" not in additional_settings.keys()) else additional_settings["end_page"]
        self.file_path = os.path.join(os.getcwd(), "flats_parse_result.csv")

    def is_sale(self):
        return self.deal_type == "sale"

    def is_rent_long(self):
        return self.deal_type == "rent" and self.rent_period_type == 4

    def is_rent_short(self):
        return self.deal_type == "rent" and self.rent_period_type == 2


    def define_average_price(self, price_data):
        if "price" in price_data:
            self.average_price = (self.average_price * self.count_parsed_offers + price_data["price"]) / self.count_parsed_offers
        elif "price_per_month" in price_data:
            self.average_price = (self.average_price * self.count_parsed_offers + price_data["price_per_month"]) / self.count_parsed_offers

    def print_parse_progress(self, page_number, count_of_pages, offers, ind):
        progress = math.ceil((ind + 1) * 100 / len(offers))
        sys.stdout.write(f"\rProcessing {page_number} page / {count_of_pages} pages: {progress}% ({ind + 1}/{len(offers)})")
        sys.stdout.flush()
        if ind == len(offers) - 1:
            print()

    def remove_unnecessary_fields(self):
        if self.is_sale():
            for not_need_field in SPECIFIC_FIELDS_FOR_RENT_LONG:
                if not_need_field in self.result[-1]:
                    del self.result[-1][not_need_field]

            for not_need_field in SPECIFIC_FIELDS_FOR_RENT_SHORT:
                if not_need_field in self.result[-1]:
                    del self.result[-1][not_need_field]

        if self.is_rent_long():
            for not_need_field in SPECIFIC_FIELDS_FOR_RENT_SHORT:
                if not_need_field in self.result[-1]:
                    del self.result[-1][not_need_field]

            for not_need_field in SPECIFIC_FIELDS_FOR_SALE:
                if not_need_field in self.result[-1]:
                    del self.result[-1][not_need_field]

        if self.is_rent_short():
            for not_need_field in SPECIFIC_FIELDS_FOR_RENT_LONG:
                if not_need_field in self.result[-1]:
                    del self.result[-1][not_need_field]

            for not_need_field in SPECIFIC_FIELDS_FOR_SALE:
                if not_need_field in self.result[-1]:
                    del self.result[-1][not_need_field]

        return self.result


    def save_results(self):
        self.remove_unnecessary_fields()

        if not self.result:
            print("Нет новых данных для сохранения")
            return

        file_path = '/Users/nikitavolnuhin/ghostbusters/parsers/flats_parse_result.csv'

        self._remove_duplicates_in_current_batch()

        existing_hashes = set()
        if Path(file_path).exists():
            with open(file_path, 'r', encoding='utf-8') as f:
                reader = csv.DictReader(f, delimiter=';')
                existing_hashes = {self._get_record_hash(row) for row in reader}

        new_records = [
            record for record in self.result
            if self._get_record_hash(record) not in existing_hashes
        ]

        if not new_records:
            print("Все записи уже существуют в файле")
            self.result = []
            return

        keys = new_records[0].keys()
        with open(file_path, 'a', newline='', encoding='utf-8') as output_file:
            dict_writer = csv.DictWriter(output_file, keys, delimiter=';')

            if output_file.tell() == 0:
                dict_writer.writeheader()

            dict_writer.writerows(new_records)

        print(f"Добавлено {len(new_records)} новых записей")
        self.result = []

    def _get_record_hash(self, record):
        unique_str = f"{record.get('id')}_{record.get('url')}_{record.get('address')}"
        return hashlib.md5(unique_str.encode('utf-8')).hexdigest()

    def _remove_duplicates_in_current_batch(self):
        unique = {}
        for record in self.result:
            record_hash = self._get_record_hash(record)
            unique[record_hash] = record
        self.result = list(unique.values())
