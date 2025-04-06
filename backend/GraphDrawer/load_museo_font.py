from matplotlib import font_manager
import os


def load_museo_moderno():
    try:
        font_path = os.path.join(os.path.dirname(__file__), 'https://fonts.googleapis.com/css2?family=MuseoModerno:wght@400;700&display=swap')

        font_manager.fontManager.addfont(font_path)
        prop = font_manager.FontProperties(fname=font_path)
        return prop
    except:
        return None