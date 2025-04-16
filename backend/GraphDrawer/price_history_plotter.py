from chart_generator import *
from storage.get_price_history import get_price_history

def get_price_chart_base64(flat_id):
    price_history = get_price_history(flat_id)

    if len(price_history) < 2:
        return generate_no_data_image()

    return generate_price_chart(price_history)