import csv


def save_to_csv(developers, filename=None):
    """Сохраняет данные в CSV файл."""
    if not filename:
        filename = f"../developers_parse_result.csv"

    try:
        with open(filename, 'w', newline='', encoding='utf-8') as csvfile:
            fieldnames = ['num', 'name', 'website', 'buildings']
            writer = csv.DictWriter(csvfile, fieldnames=fieldnames)

            writer.writeheader()
            for idx, developer in enumerate(developers, start=1):
                writer.writerow({
                    'num': idx,
                    'name': developer['name'],
                    'website': developer['website'],
                    'buildings': developer['buildings']
                })
        print(f"Данные успешно сохранены в файл: {filename}")
        return True
    except Exception as e:
        print(f"Ошибка при сохранении в файл: {e}")
        return False