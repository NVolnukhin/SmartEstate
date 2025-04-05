from bs4 import BeautifulSoup
from selenium import webdriver

def parse_cian_images(dict_for_parse: dict[int, str], batch_size=None) -> dict[int, set]:
    try:
        images_dict = {}
        driver = webdriver.Chrome()
        items_to_process = dict_for_parse.items()
        if batch_size is not None:
            items_to_process = list(dict_for_parse.items())[:batch_size]

        for idx, urlx in items_to_process:
            try:
                driver.get(urlx)
                soup = BeautifulSoup(driver.page_source, 'html.parser')

                images = set()

                main_img_tag = soup.find('li', class_='a10a3f92e9--container--Havpv').find('img')
                if main_img_tag and main_img_tag.get('src'):
                    images.add(main_img_tag['src'])

                floor_plan_div = soup.find('div', {'data-name': 'FloorPlan'})
                if floor_plan_div:
                    floor_plan_img = floor_plan_div.find('img')
                    if floor_plan_img and floor_plan_img.get('src'):
                        images.add(floor_plan_img['src'])

                other_images_divs = soup.find_all('div', class_='a10a3f92e9--container--zARIJ')
                print(f"Flat Id : {idx}")
                for div in other_images_divs:
                    img_tag = div.find('img')
                    if img_tag and img_tag.get('src'):
                        img = img_tag['src']
                        print(f"{img}")
                        images.add(img_tag['src'])
                print()

                images_dict[idx] = images

            except Exception as e:
                print(f"Ошибка при обработке квартиры {idx}: {e}")
                continue

        driver.quit()
        return images_dict

    except Exception as e:
        print(f"Произошла ошибка: {e}")
        if 'driver' in locals():
            driver.quit()
        return {}