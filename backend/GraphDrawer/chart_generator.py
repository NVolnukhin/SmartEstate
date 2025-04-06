import matplotlib.pyplot as plt
from io import BytesIO
import base64
from matplotlib import font_manager
from load_museo_font import load_museo_moderno


def generate_no_data_image():
    museo_prop = load_museo_moderno()

    plt.figure(figsize=(8, 4), facecolor='white')
    plt.text(0.5, 0.5, 'История изменения цен не найдена',
             ha='center', va='center',
             fontsize=16, color='#40027E',
             fontproperties=museo_prop if museo_prop else None)
    plt.axis('off')

    buf = BytesIO()
    plt.savefig(buf, format='png', bbox_inches='tight', pad_inches=0.1, dpi=100)
    plt.close()
    buf.seek(0)
    return base64.b64encode(buf.read()).decode('utf-8')



def generate_price_chart(price_history):
    dates = [entry['date'] for entry in price_history]
    prices = [float(entry['price']) for entry in price_history]

    min_price = min(prices)
    max_price = max(prices)

    padding = (max_price - min_price) * 0.1

    plt.figure(figsize=(10, 6), facecolor='#FFE4AA')

    ax = plt.gca()
    ax.set_facecolor('#FFE4AA')

    line, = plt.plot(dates, prices, marker='o', color='#40027E', linewidth=2, markersize=8)

    plt.fill_between(dates, prices, color='#40027E', alpha=0.1)

    plt.ylim(
        max(0, min_price - padding),
        max_price + padding
    )

    plt.xlim(dates[0], dates[-1])  # От первой до последней даты

    plt.title('История изменения цены', pad=20, fontsize=16,
              fontproperties=font_manager.FontProperties(family='MuseoModerno'))
    plt.xlabel('Дата', labelpad=10)
    plt.ylabel('Цена, руб.', labelpad=10)
    plt.grid(True, linestyle='--', alpha=0.7)

    plt.gca().yaxis.set_major_formatter(plt.matplotlib.ticker.StrMethodFormatter('{x:,.0f}'))

    plt.xticks(rotation=45)

    plt.tight_layout()

    buf = BytesIO()
    plt.savefig(buf, format='png', facecolor='#FFE4AA', bbox_inches='tight', dpi=200)
    plt.close()
    buf.seek(0)
    return base64.b64encode(buf.read()).decode('utf-8')
