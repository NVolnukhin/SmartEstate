from bs4 import BeautifulSoup
from selenium import webdriver

def parse_cian_images(dict_for_parse: dict[int, str]) -> dict[int, set]:
    try:
        images_dict = {}
        for idx, urlx in dict_for_parse.items():
            driver = webdriver.Chrome()
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
            print(len(other_images_divs))
            for div in other_images_divs:
                img_tag = div.find('img')
                if img_tag and img_tag.get('src'):
                    images.add(img_tag['src'])

            images_dict[idx] = images

        return images_dict

    except Exception as e:
        print(f"Произошла ошибка: {e}")
        return

