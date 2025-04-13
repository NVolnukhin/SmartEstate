import csv

def delete_if_no_year_of_construction(input_file):
    output_rows = []

    with open(input_file, 'r', encoding='utf-8') as csvfile:
        reader = csv.DictReader(csvfile, delimiter=';')
        fieldnames = reader.fieldnames

        for row in reader:
            year = row.get('year_of_construction', '').strip()
            if year:
                output_rows.append(row)

    with open(input_file, 'w', newline='', encoding='utf-8') as csvfile:
        writer = csv.DictWriter(csvfile, fieldnames=fieldnames, delimiter=';')
        writer.writeheader()
        writer.writerows(output_rows)
