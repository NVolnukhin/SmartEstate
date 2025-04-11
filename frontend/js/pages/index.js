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
                        ${apartment.roominess}-комнатная<br>
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
                window.location.href = `./flat.html?id=${apartmentId}`;
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
                    window.location.href = './login.html';
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