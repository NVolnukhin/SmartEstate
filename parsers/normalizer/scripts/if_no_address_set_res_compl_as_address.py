import csv

def if_no_address_set_res_compl_as_address(input_file):
    output_rows = []

    with open(input_file, 'r', encoding='utf-8') as csvfile:
        reader = csv.DictReader(csvfile, delimiter=';')
        fieldnames = reader.fieldnames

        for row in reader:
            if not row.get('street', '').strip():
                row['street'] = row.get('residential_complex', '').strip()
            output_rows.append(row)

    with open(input_file, 'w', newline='', encoding='utf-8') as csvfile:
        writer = csv.DictWriter(csvfile, fieldnames=fieldnames, delimiter=';')
        writer.writeheader()
        writer.writerows(output_rows)