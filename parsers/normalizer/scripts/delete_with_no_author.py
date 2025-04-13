import csv

def delete_with_no_author(input_file):
    output_rows = []

    with open(input_file, 'r', encoding='utf-8') as csvfile:
        reader = csv.DictReader(csvfile, delimiter=';')
        fieldnames = reader.fieldnames

        for row in reader:
            author = row.get('author', '').strip()
            if author and not author.startswith('ID '):
                output_rows.append(row)

    with open(input_file, 'w', newline='', encoding='utf-8') as csvfile:
        writer = csv.DictWriter(csvfile, fieldnames=fieldnames, delimiter=';')
        writer.writeheader()
        writer.writerows(output_rows)
