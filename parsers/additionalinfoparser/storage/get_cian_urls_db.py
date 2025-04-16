import psycopg2

def get_links(db_config) -> dict[int, str]:
    conn = psycopg2.connect(**db_config)
    cursor = conn.cursor()

    cursor.execute("""
    SELECT "FlatId", "CianLink" FROM "Flats"
    """)
    results = cursor.fetchall()

    flats_dict = {}
    for row in results:
        flats_dict[row[0]] = row[1]

    conn.close()

    return flats_dict

def get_null_img_links(db_config) -> dict[int, str]:
    conn = psycopg2.connect(**db_config)
    cursor = conn.cursor()

    cursor.execute("""
    SELECT "FlatId", "CianLink" 
    FROM "Flats"
    WHERE "Images" = ''
    ORDER BY "FlatId" DESC
    """)
    results = cursor.fetchall()

    flats_dict = {}
    for row in results:
        flats_dict[row[0]] = row[1]

    conn.close()

    return flats_dict