import { initTokenChecker, checkTokenExpiration } from '../components/auth/token_сhecker.js';
import { config } from '../config.js'

document.addEventListener('DOMContentLoaded', function() {     
    initTokenChecker();

    function checkAuth() {
        return localStorage.getItem('authToken') !== null && checkTokenExpiration();
    }
    
    function updateUI() {
        if(checkAuth()) {
            document.querySelector('.header-link').style.display = 'none';
            document.querySelector('.profile-link-wrapper').style.display = 'block';
        } else {
            document.querySelector('.header-link').style.display = 'flex';
            document.querySelector('.profile-link-wrapper').style.display = 'none';
        }
    }
    
    updateUI();

    async function loadApartments() {
        try {
            const response = await fetch(`${config.api.baseUrl}/flats/random?count=10`);
            const apartments = await response.json();
            
            if (response.ok) {
                renderApartments(apartments);
            } else {
                console.error('Ошибка загрузки квартир:', apartments.message);
                renderError();
            }
        } catch (error) {
            console.error('Ошибка соединения:', error);
            renderError();
        }
    }

    function renderApartments(apartments) {
        const container = document.getElementById('apartmentsContainer');
        container.innerHTML = '';
        
        apartments.forEach(apartment => {
            const card = document.createElement('div');
            card.className = 'apartment-card';
            card.setAttribute('data-id', apartment.flatId);
            
            const formattedPrice = new Intl.NumberFormat('ru-RU', {
                style: 'currency',
                currency: 'RUB',
                maximumFractionDigits: 0
            }).format(apartment.price).replace('RUB', '₽');
            
            card.innerHTML = `
                <div class="apartment-image">
                    ${apartment.mainImage ? 
                        `<img src="${apartment.mainImage}" alt="Квартира ${apartment.flatId}">` : 
                        `ЖК ${apartment.flatId}<br>${apartment.nearestMetro.name}`
                    }
                </div>
                <div class="apartment-price">
                    ${formattedPrice}
                    <button class="favorite-btn" data-id="${apartment.flatId}">
                        <i class="far fa-heart"></i>
                    </button>
                </div>
                <div class="apartment-details">
                    <div class="apartment-name">ЖК "${apartment.residentialComplex}"</div>
                    <div class="apartment-metro">
                        <i class="fas fa-subway"></i>
                        ${apartment.nearestMetro.name} (${apartment.nearestMetro.minutesToMetro} мин)
                    </div>
                    <div>
                        ${apartment.roominess === -1 ? 'Студия' : apartment.roominess === -2 ? 'Своб. планировка' : `${apartment.roominess}-комнатная`} <br>
                        ${apartment.square} м²<br>
                        Этаж ${apartment.floor}
                    </div>
                </div>
            `;
            
            container.appendChild(card);
        });

        document.querySelectorAll('.apartment-card').forEach(card => {
            card.addEventListener('click', function(e) {
                if (e.target.closest('.favorite-btn')) {
                    e.stopPropagation();
                    return;
                }
                
                const apartmentId = this.getAttribute('data-id');
                window.location.href = `./flat?id=${apartmentId}`;
            });
        });
        
        initFavoriteButtons();
    }

    function renderError() {
        const container = document.getElementById('apartmentsContainer');
        container.innerHTML = `
            <div style="color: #FFE4AA; text-align: center; width: 100%; padding: 20px;">
                Не удалось загрузить список квартир. Пожалуйста, попробуйте позже.
            </div>
        `;
    }

    function initFavoriteButtons() {
        const isAuth = checkAuth();
        
        if (isAuth) {
            loadFavorites().then(favorites => {
                console.log('Favorites loaded:', favorites);
                updateFavoriteIcons(favorites);
            });
        }
        
        document.querySelectorAll('.favorite-btn').forEach(btn => {
            btn.addEventListener('click', async function(e) {
                e.preventDefault();
                e.stopPropagation();
                
                if (!isAuth) {
                    window.location.href = './login';
                    return;
                }
                
                const flatId = this.getAttribute('data-id');
                const icon = this.querySelector('i');
                
                if (icon.classList.contains('fas')) {
                    const success = await removeFromFavorites(flatId);
                    if (success) {
                        icon.classList.remove('fas');
                        icon.classList.add('far');
                    }
                } else {
                    const success = await addToFavorites(flatId);
                    if (success) {
                        icon.classList.remove('far');
                        icon.classList.add('fas');
                    }
                }
            });
        });
    }            

    function updateFavoriteIcons(favorites) {
        console.log("Избранные ID:", favorites);
        document.querySelectorAll('.favorite-btn').forEach(btn => {
            const flatId = btn.getAttribute('data-id');
            const icon = btn.querySelector('i');
            
            if (favorites.includes(flatId)) {
                icon.classList.replace('far', 'fas');
            } else {
                icon.classList.replace('fas', 'far');
            }
        });
    }
    
    async function loadFavorites() {
        try {
            const response = await fetch(`${config.api.baseUrl}/user-preferences/favorites`, {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                }
            });
            
            if (!response.ok) {
                console.error('Ошибка загрузки избранного:', await response.text());
                return [];
            }
    
            const data = await response.json();
            console.log("Ответ API (избранное):", data);
            
            if (Array.isArray(data)) {
                return data.map(item => String(item.flatId));
            }
            
            console.error('Неизвестный формат избранного:', data);
            return [];
        } catch (error) {
            console.error('Ошибка загрузки избранного:', error);
            return [];
        }
    }

    async function addToFavorites(flatId) {
        try {
            const response = await fetch(`${config.api.baseUrl}/user-preferences/favorites`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                },
                body: JSON.stringify({ flatId: parseInt(flatId) })
            });
            
            return response.ok;
        } catch (error) {
            console.error('Ошибка добавления в избранное:', error);
            return false;
        }
    }

    async function removeFromFavorites(flatId) {
        try {
            const response = await fetch(`${config.api.baseUrl}/user-preferences/favorites/${flatId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                }
            });
            
            return response.ok;
        } catch (error) {
            console.error('Ошибка удаления из избранного:', error);
            return false;
        }
    }

    const mapModal = document.getElementById('mapModal');
    const showMapButton = document.getElementById('showMapButton');
    const closeModal = document.querySelector('.close-modal');
    let map;

    showMapButton.addEventListener('click', function() {
        mapModal.style.display = 'block';
        document.body.style.overflow = 'hidden';
        
        if (!map) {
            const script = document.createElement('script');
            script.src = `https://api-maps.yandex.ru/2.1/?apikey=${config.api.ymaps_api_key}&lang=ru_RU`;
            script.onload = initMap;
            document.head.appendChild(script);
        }
    });

    closeModal.addEventListener('click', function() {
        mapModal.style.display = 'none';
        document.body.style.overflow = '';
    });

    window.addEventListener('click', function(event) {
        if (event.target === mapModal) {
            mapModal.style.display = 'none';
            document.body.style.overflow = '';
        }
    });

    function initMap() {
        ymaps.ready(function() {
            map = new ymaps.Map('mapContainer', {
                center: [55.751244, 37.618423],
                zoom: 11,
                controls: ['zoomControl']
            });
    
            map.options.set({
                suppressMapOpenBlock: true
            });
            
            map.controls.get('zoomControl').options.set({
                size: 'small',
                position: { right: 10, top: 100 }
            });
    
            loadBuildings();
        });
    }    

    async function loadBuildings() {
        try {
            const response = await fetch(`${config.api.baseUrl}/buildings`);
            const buildings = await response.json();
            
            if (response.ok) {
                renderBuildingsOnMap(buildings);
            } else {
                console.error('Ошибка загрузки зданий:', buildings.message);
            }
        } catch (error) {
            console.error('Ошибка соединения:', error);
        }
    }    

    function renderBuildingsOnMap(buildings) {
        const collection = new ymaps.GeoObjectCollection(null, {
            preset: 'islands#darkOrangeDotIcon',
            openBalloonOnClick: false
        });
    
        buildings.forEach(building => {
            const placemark = new ymaps.Placemark(
                [building.latitude, building.longitude],
                {
                    hintContent: building.residentalComplex,
                    balloonContent: `<strong>${building.residentalComplex}</strong><br>Квартир: ${building.flatsCount}`,
                },
                {
                    preset: 'islands#darkOrangeDotIcon'
                }
            );
    
            placemark.events.add('click', async () => {
                await showFlatsInBuilding(building.buildingId, building.residentalComplex);
            });
    
            collection.add(placemark);
        });
    
        map.geoObjects.add(collection);
    }
    

    async function showFlatsInBuilding(buildingId, complexName) {
        const sidebar = document.getElementById('flatsSidebar');
        sidebar.innerHTML = `<h3 style="color: #FFE4AA; margin-top: 0;">${complexName}</h3>`;
        
        try {
            const response = await fetch(`${config.api.baseUrl}/buildings/${buildingId}/flats`);
            const flats = await response.json();
            
            if (response.ok) {
                renderFlatsInSidebar(flats);
            } else {
                sidebar.innerHTML += `<p style="color: #FFE4AA;">Не удалось загрузить квартиры</p>`;
            }
        } catch (error) {
            console.error('Ошибка загрузки квартир:', error);
            sidebar.innerHTML += `<p style="color: #FFE4AA;">Ошибка загрузки квартир</p>`;
        }
    }

    function renderFlatsInSidebar(flats) {
        const sidebar = document.getElementById('flatsSidebar');
        
        if (flats.length === 0) {
            sidebar.innerHTML += `<p style="color: #FFE4AA;">Нет доступных квартир в этом здании</p>`;
            return;
        }
        
        flats.forEach(flat => {
            const formattedPrice = new Intl.NumberFormat('ru-RU', {
                style: 'currency',
                currency: 'RUB',
                maximumFractionDigits: 0
            }).format(flat.price).replace('RUB', '₽');
            
            const roominessText = flat.roominess === -1 ? 'Студия' : 
                               flat.roominess === -2 ? 'Своб. планировка' : 
                               `${flat.roominess}-комнатная`;
            
            const flatCard = document.createElement('div');
            flatCard.className = 'flat-card-sidebar';
            flatCard.innerHTML = `
                <h4>${flat.residentialComplex}</h4>
                <div class="price">${formattedPrice}</div>
                <div class="details">
                    <span>${roominessText}</span>
                    <span>${flat.square} м²</span>
                    <span>Этаж ${flat.floor}</span>
                </div>
                <div class="metro">
                    <i class="fas fa-subway"></i> ${flat.nearestMetro.name} (${flat.nearestMetro.minutesToMetro} мин)
                </div>
            `;
            
            flatCard.addEventListener('click', () => {
                window.location.href = `./flat?id=${flat.flatId}`;
            });
            
            sidebar.appendChild(flatCard);
        });
    }

    loadApartments();

    const cards = document.querySelectorAll('.developer-card');
    cards.forEach(card => {
        card.addEventListener('click', function() {
            this.style.transform = 'scale(0.95)';
            setTimeout(() => {
                this.style.transform = '';
            }, 200);
        });
    });
    
    const screenElement = document.querySelector('.screen');
    let mouseX = 0;
    let mouseY = 0;
    
    document.addEventListener('mousemove', (e) => {
        mouseX = (e.clientX / window.innerWidth - 0.5) * 30;
        mouseY = (e.clientY / window.innerHeight - 0.5) * 30;
        
        screenElement.style.backgroundPosition = `${50 + mouseX}% ${50 + mouseY}%`;
    });
});