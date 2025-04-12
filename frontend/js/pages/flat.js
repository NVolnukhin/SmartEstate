import { initTokenChecker, checkTokenExpiration } from '../components/auth/token_сhecker.js';
import { showNotification, initNotification } from '../components/ui/notification.js';
import { config } from '../config.js';

document.addEventListener('DOMContentLoaded', function() {
    initTokenChecker();
    initNotification();

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

    function loadYandexMaps() {
        if (window.ymaps && window.ymaps.ready) {
            return Promise.resolve(ymaps);
        }
        
        if (window.ymapsLoadingPromise) {
            return window.ymapsLoadingPromise;
        }
    
        window.ymapsLoadingPromise = new Promise((resolve, reject) => {
            const script = document.createElement('script');
            script.src = `https://api-maps.yandex.ru/2.1/?apikey=${config.api.ymaps_api_key}&lang=ru_RU`;
            script.type = 'text/javascript';
            
            script.onload = () => {
                ymaps.ready(() => {
                    console.log('Yandex Maps API loaded and ready');
                    resolve(ymaps);
                });
            };
            
            script.onerror = (error) => {
                console.error('Failed to load Yandex Maps API:', error);
                reject(new Error('Не удалось загрузить Яндекс.Карты'));
                delete window.ymapsLoadingPromise;
            };
            
            document.head.appendChild(script);
        });
    
        return window.ymapsLoadingPromise;
    }

    const urlParams = new URLSearchParams(window.location.search);
    const apartmentId = urlParams.get('id');
    
    if (!apartmentId) {
        window.location.href = 'all_flats';
        return;
    }

    async function loadApartmentData() {
        const loadingIndicator = document.getElementById('loading-indicator');
        try {
            loadingIndicator.style.display = 'block';
            document.getElementById('apartment-name').textContent = 'Загрузка данных...';
            
            const [response, ymaps] = await Promise.all([
                fetch(`${config.api.baseUrl}/flats/${apartmentId}`, {
                    headers: checkAuth() ? {
                        'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                    } : {}
                }),
                loadYandexMaps()
            ]);
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const data = await response.json();
            
            if (!data || !data.buildingInfo || !data.buildingInfo.residentialComplex) {
                throw new Error('Неполные данные о квартире');
            }
            
            renderApartmentData(data);
            initPhotoSlider(data.images);
            
            if (data.buildingInfo.coordinates || data.nearestMetroInfo?.nearestMetroCoordinates) {
                initMap(data, ymaps);
            } else {
                document.getElementById('map').innerHTML = 'Координаты не указаны';
            }    

            if (data.priceChart && Array.isArray(data.priceChart)) {
                console.log('Price chart data received:', data.priceChart);
                renderPriceTooltip(data.priceChart);
                renderFullPriceChart(data.priceChart);
            } else {
                console.log('No price chart data');
                document.getElementById('price-tooltip-content').textContent = 'Данные об изменении цен отсутствуют';
                document.getElementById('price-chart-full').textContent = 'Не найдено изменений цен';
            }
            
        } catch (error) {
            console.error('Ошибка загрузки данных:', error);
            
            let errorMessage = 'Ошибка загрузки данных';
            if (error.message.includes('Failed to fetch')) {
                errorMessage = 'Ошибка соединения с сервером';
            } else if (error.message.includes('HTTP error')) {
                errorMessage = 'Квартира не найдена';
            } else if (error.message.includes('Неполные данные')) {
                errorMessage = 'Неполные данные о квартире';
            }
            
            showNotification(errorMessage);
        } finally {
            loadingIndicator.style.display = 'none';
        }
    }

    function renderApartmentData(data) {
        document.getElementById('apartment-name').textContent = `ЖК ${data.buildingInfo.residentialComplex}`;
        document.getElementById('apartment-description').textContent = 
            `${data.roominess}-комнатная квартира ${data.square} м² на ${data.floor} этаже`;
        
        const formattedPrice = new Intl.NumberFormat('ru-RU', {
            style: 'currency',
            currency: 'RUB',
            maximumFractionDigits: 0
        }).format(data.price).replace('RUB', '₽');
        
        document.getElementById('price-value').textContent = formattedPrice;
        document.getElementById('apartment-square').textContent = `${data.square} м²`;
        document.getElementById('apartment-rooms').textContent = `${data.roominess}-комнатная`;
        document.getElementById('apartment-floor').textContent = `${data.floor}/${data.buildingInfo.floorCount}`;
        document.getElementById('apartment-finish').textContent = data.finishType;
        
        const cianLink = document.getElementById('cian-link');
        cianLink.textContent = 'Перейти на Cian';
        cianLink.href = data.cianLink || '#';
        
        document.getElementById('building-status').textContent = data.buildingInfo.status;
        document.getElementById('building-floors').textContent = data.buildingInfo.floorCount;
        document.getElementById('building-address').textContent = data.buildingInfo.address;
        document.getElementById('building-complex').textContent = data.buildingInfo.residentialComplex;
        
        document.getElementById('metro-info').textContent = 
            `${data.nearestMetroInfo.name} (${data.nearestMetroInfo.minutesToMetro} мин)`;
        document.getElementById('school-info').textContent = 
            `${data.nearestSchoolInfo.name} (${data.nearestSchoolInfo.minutesToSchool} мин)`;
        document.getElementById('kindergarten-info').textContent = 
            `${data.nearestKindergartenInfo.name} (${data.nearestKindergartenInfo.minutesToKindergarten} мин)`;
        document.getElementById('shop-info').textContent = 
            `${data.nearestShopInfo.name} (${data.nearestShopInfo.minutesToShop} мин)`;
        document.getElementById('pharmacy-info').textContent = 
            `${data.nearestPharmacyInfo.name} (${data.nearestPharmacyInfo.minutesToPharmacy} мин)`;
        
        document.getElementById('developer-name').textContent = data.developerInfo.name;
        document.getElementById('developer-buildings').textContent = data.developerInfo.buildingsCount;
        const devWebsite = document.getElementById('developer-website');
        devWebsite.textContent = data.developerInfo.website;
        devWebsite.href = data.developerInfo.website || '#';
    }

    function initPhotoSlider(images) {
        const photoContainer = document.getElementById('apartment-photo');
        const controlsContainer = document.getElementById('photo-controls');
        
        if (!images || images.length === 0) {
            const placeholder = document.createElement('div');
            placeholder.className = 'no-photos';
            placeholder.textContent = 'Фотографии отсутствуют';
            placeholder.style.display = 'flex';
            placeholder.style.alignItems = 'center';
            placeholder.style.justifyContent = 'center';
            placeholder.style.height = '100%';
            placeholder.style.color = '#40027E';
            photoContainer.appendChild(placeholder);
            return;
        }
        
        const fullscreenOverlay = document.createElement('div');
        fullscreenOverlay.className = 'fullscreen-overlay';
        
        const fullscreenContent = document.createElement('div');
        fullscreenContent.className = 'fullscreen-content';
        
        const fullscreenImg = document.createElement('img');
        fullscreenImg.className = 'fullscreen-img';
        
        const fullscreenClose = document.createElement('div');
        fullscreenClose.className = 'fullscreen-close';
        fullscreenClose.innerHTML = '<i class="fas fa-times"></i>';
        
        const fullscreenFavorite = document.createElement('div');
        fullscreenFavorite.className = 'fullscreen-favorite';
        fullscreenFavorite.innerHTML = '<i class="far fa-heart"></i>';
        
        const fullscreenPrev = document.createElement('div');
        fullscreenPrev.className = 'fullscreen-nav fullscreen-prev';
        fullscreenPrev.innerHTML = '<i class="fas fa-chevron-left"></i>';
        
        const fullscreenNext = document.createElement('div');
        fullscreenNext.className = 'fullscreen-nav fullscreen-next';
        fullscreenNext.innerHTML = '<i class="fas fa-chevron-right"></i>';
        
        const fullscreenDots = document.createElement('div');
        fullscreenDots.className = 'fullscreen-dots';
        
        fullscreenContent.appendChild(fullscreenImg);
        fullscreenContent.appendChild(fullscreenClose);
        fullscreenContent.appendChild(fullscreenFavorite);
        fullscreenContent.appendChild(fullscreenPrev);
        fullscreenContent.appendChild(fullscreenNext);
        fullscreenContent.appendChild(fullscreenDots);
        fullscreenOverlay.appendChild(fullscreenContent);
        document.body.appendChild(fullscreenOverlay);
        
        let currentIndex = 0;
        const photos = [];
        
        images.forEach((img, index) => {
            const imgElement = document.createElement('img');
            imgElement.src = img;
            imgElement.alt = `Фото квартиры ${index + 1}`;
            if (index === 0) imgElement.classList.add('active');
            imgElement.style.cursor = 'pointer';
            photoContainer.appendChild(imgElement);
            photos.push(imgElement);
            
            const dot = document.createElement('div');
            dot.className = 'photo-dot';
            if (index === 0) dot.classList.add('active');
            dot.setAttribute('data-index', index);
            controlsContainer.appendChild(dot);
            
            const fullscreenDot = document.createElement('div');
            fullscreenDot.className = 'fullscreen-dot';
            if (index === 0) fullscreenDot.classList.add('active');
            fullscreenDot.setAttribute('data-index', index);
            fullscreenDots.appendChild(fullscreenDot);
        });
        
        function showPhoto(index) {
            photos.forEach((photo, i) => {
                photo.classList.toggle('active', i === index);
            });
            
            if (fullscreenOverlay.style.display === 'flex') {
                fullscreenImg.src = images[index];
            }
            
            const dots = controlsContainer.querySelectorAll('.photo-dot');
            dots.forEach((dot, i) => {
                dot.classList.toggle('active', i === index);
            });
            
            const fullscreenDotsElements = fullscreenDots.querySelectorAll('.fullscreen-dot');
            fullscreenDotsElements.forEach((dot, i) => {
                dot.classList.toggle('active', i === index);
            });
            
            currentIndex = index;
        }
        
        document.querySelector('.photo-prev').addEventListener('click', () => {
            const newIndex = (currentIndex - 1 + photos.length) % photos.length;
            showPhoto(newIndex);
        });
        
        document.querySelector('.photo-next').addEventListener('click', () => {
            const newIndex = (currentIndex + 1) % photos.length;
            showPhoto(newIndex);
        });
        
        fullscreenPrev.addEventListener('click', () => {
            const newIndex = (currentIndex - 1 + photos.length) % photos.length;
            showPhoto(newIndex);
        });
        
        fullscreenNext.addEventListener('click', () => {
            const newIndex = (currentIndex + 1) % photos.length;
            showPhoto(newIndex);
        });
        
        controlsContainer.querySelectorAll('.photo-dot').forEach(dot => {
            dot.addEventListener('click', () => {
                const index = parseInt(dot.getAttribute('data-index'));
                showPhoto(index);
            });
        });
        
        fullscreenDots.querySelectorAll('.fullscreen-dot').forEach(dot => {
            dot.addEventListener('click', () => {
                const index = parseInt(dot.getAttribute('data-index'));
                showPhoto(index);
            });
        });
        
        photos.forEach(img => {
            img.addEventListener('click', () => {
                fullscreenImg.src = images[currentIndex];
                fullscreenOverlay.style.display = 'flex';
                document.body.style.overflow = 'hidden';
            });
        });
        
        fullscreenClose.addEventListener('click', () => {
            fullscreenOverlay.style.display = 'none';
            document.body.style.overflow = '';
        });
        
        fullscreenOverlay.addEventListener('click', (e) => {
            if (e.target === fullscreenOverlay) {
                fullscreenOverlay.style.display = 'none';
                document.body.style.overflow = '';
            }
        });
        
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && fullscreenOverlay.style.display === 'flex') {
                fullscreenOverlay.style.display = 'none';
                document.body.style.overflow = '';
            }
        });
        
        fullscreenFavorite.addEventListener('click', async function(e) {
            e.stopPropagation();
            
            if (!checkAuth()) {
                window.location.href = 'login';
                return;
            }
            
            const icon = this.querySelector('i');
            const isFavorite = icon.classList.contains('fas');
            const mainFavoriteIcon = document.querySelector('.favorite-btn i');
            
            try {
                let success;
                if (isFavorite) {
                    success = await removeFromFavorites(apartmentId);
                } else {
                    success = await addToFavorites(apartmentId);
                }
                
                if (success) {
                    if (isFavorite) {
                        icon.classList.remove('fas');
                        icon.classList.add('far');
                        mainFavoriteIcon.classList.remove('fas');
                        mainFavoriteIcon.classList.add('far');
                    } else {
                        icon.classList.remove('far');
                        icon.classList.add('fas');
                        mainFavoriteIcon.classList.remove('far');
                        mainFavoriteIcon.classList.add('fas');
                    }
                }
            } catch (error) {
                console.error('Ошибка обновления избранного:', error);
            }
        });
        
        let slideInterval;

        function startAutoSlide() {
            stopAutoSlide();
            slideInterval = setInterval(() => {
                const newIndex = (currentIndex + 1) % photos.length;
                showPhoto(newIndex);
            }, 3000);
        }

        function stopAutoSlide() {
            if (slideInterval) {
                clearInterval(slideInterval);
                slideInterval = null;
            }
        }

        startAutoSlide();

        photoContainer.addEventListener('mouseenter', stopAutoSlide);
        photoContainer.addEventListener('mouseleave', startAutoSlide);

        fullscreenOverlay.addEventListener('mouseenter', stopAutoSlide);
        fullscreenOverlay.addEventListener('mouseleave', startAutoSlide);

        document.querySelector('.photo-prev').addEventListener('click', () => {
            stopAutoSlide();
            setTimeout(startAutoSlide, 10000);
        });

        document.querySelector('.photo-next').addEventListener('click', () => {
            stopAutoSlide();
            setTimeout(startAutoSlide, 10000);
        });

        fullscreenPrev.addEventListener('click', () => {
            stopAutoSlide();
            setTimeout(startAutoSlide, 10000);
        });

        fullscreenNext.addEventListener('click', () => {
            stopAutoSlide();
            setTimeout(startAutoSlide, 10000);
        });
        
        if (checkAuth()) {
            const mainFavoriteIcon = document.querySelector('.favorite-btn i');
            const fullscreenFavoriteIcon = fullscreenFavorite.querySelector('i');
            
            fullscreenFavoriteIcon.className = mainFavoriteIcon.className;
        }
        
        function handleKeyDown(e) {
            if (fullscreenOverlay.style.display !== 'flex') return;
            
            switch(e.key) {
                case 'ArrowLeft':
                    const prevIndex = (currentIndex - 1 + photos.length) % photos.length;
                    showPhoto(prevIndex);
                    e.preventDefault();
                    break;
                case 'ArrowRight':
                    const nextIndex = (currentIndex + 1) % photos.length;
                    showPhoto(nextIndex);
                    e.preventDefault();
                    break;
                case 'Escape':
                    fullscreenOverlay.style.display = 'none';
                    document.body.style.overflow = '';
                    e.preventDefault();
                    break;
            }
        }
    
        photos.forEach(img => {
            img.addEventListener('click', () => {
                fullscreenImg.src = images[currentIndex];
                fullscreenOverlay.style.display = 'flex';
                document.body.style.overflow = 'hidden';
                document.addEventListener('keydown', handleKeyDown);
            });
        });
    
        const closeFullscreen = () => {
            fullscreenOverlay.style.display = 'none';
            document.body.style.overflow = '';
            document.removeEventListener('keydown', handleKeyDown);
        };
    
        fullscreenClose.addEventListener('click', closeFullscreen);
        fullscreenOverlay.addEventListener('click', (e) => {
            if (e.target === fullscreenOverlay) {
                closeFullscreen();
            }
        });
    }

    function renderPriceTooltip(priceChart) {
        const tooltipContent = document.getElementById('price-tooltip-content');
        
        if (!priceChart || !Array.isArray(priceChart) || priceChart.length === 0) {
            tooltipContent.textContent = 'Данные об изменении цен отсутствуют';
            return;
        }
        
        const sortedHistory = [...priceChart]
            .map(item => ({
                date: new Date(item.date),
                price: Number(item.price)
            }))
            .sort((a, b) => b.date - a.date)
            .slice(0, 5); 
        
        let html = `
            <div style="max-height: 300px; overflow-y: auto;">
                <table class="price-change-table">
                    <thead>
                        <tr>
                            <th style="width: 35%;">Дата</th>
                            <th style="width: 35%;">Цена</th>
                            <th style="width: 30%;">Изменение</th>
                        </tr>
                    </thead>
                    <tbody>`;
        
        for (let i = 0; i < sortedHistory.length; i++) {
            const entry = sortedHistory[i];
            const date = entry.date.toLocaleDateString('ru-RU', {
                day: 'numeric',
                month: 'short',
                year: 'numeric'
            });
            
            const formattedPrice = new Intl.NumberFormat('ru-RU', {
                style: 'currency',
                currency: 'RUB',
                maximumFractionDigits: 0
            }).format(entry.price).replace('RUB', '₽');
            
            let changeHtml = '';
            if (i < sortedHistory.length - 1) {
                const prevEntry = sortedHistory[i + 1];
                const change = entry.price - prevEntry.price;
                
                if (change !== 0) {
                    const absChange = Math.abs(change);
                    const formattedChange = new Intl.NumberFormat('ru-RU', {
                        style: 'currency',
                        currency: 'RUB',
                        maximumFractionDigits: 0
                    }).format(absChange).replace('RUB', '₽');
                    
                    const changeClass = change > 0 ? 'price-up' : 'price-down';
                    const arrow = change > 0 ? '↑' : '↓';
                    
                    changeHtml = `
                        <span class="${changeClass}" style="white-space: nowrap;">
                            ${arrow} ${formattedChange}
                        </span>`;
                } else {
                    changeHtml = '<span style="color: #999;">—</span>';
                }
            }
            
            html += `
                <tr>
                    <td>${date}</td>
                    <td>${formattedPrice}</td>
                    <td>${changeHtml || '<span style="color: #999;">—</span>'}</td>
                </tr>`;
        }
        
        html += `
                    </tbody>
                </table>
            </div>`;
        
        tooltipContent.innerHTML = html;
    }

    function renderFullPriceChart(priceChart) {
        const chartContainer = document.getElementById('price-chart-full');
        chartContainer.innerHTML = '';
        
        console.log('Processing price chart:', priceChart);
        
        if (!priceChart || !Array.isArray(priceChart) || priceChart.length <= 1) {
            chartContainer.textContent = 'Не найдено изменений цен';
            return;
        }
    
        try {
            const sortedHistory = priceChart
                .map(item => ({
                    date: new Date(item.date),
                    price: Number(item.price)
                }))
                .sort((a, b) => a.date - b.date);
            
            if (sortedHistory.length <= 1) {
                chartContainer.textContent = 'Недостаточно данных для построения графика';
                return;
            }
    
            const canvas = document.createElement('canvas');
            chartContainer.appendChild(canvas);
            
            const labels = sortedHistory.map(entry => 
                entry.date.toLocaleDateString('ru-RU', {
                    day: 'numeric',
                    month: 'short',
                    year: 'numeric'
                })
            );
            
            const prices = sortedHistory.map(entry => entry.price);
            
            const changes = [];
            for (let i = 1; i < sortedHistory.length; i++) {
                changes.push(sortedHistory[i].price - sortedHistory[i-1].price);
            }
            
            new Chart(canvas, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Цена, ₽',
                        data: prices,
                        borderColor: '#40027E',
                        backgroundColor: 'rgba(64, 2, 126, 0.1)',
                        borderWidth: 2,
                        pointBackgroundColor: (context) => {
                            const index = context.dataIndex;
                            if (index === 0) return '#40027E';
                            const change = changes[index-1];
                            return change > 0 ? '#e74c3c' : change < 0 ? '#2ecc71' : '#40027E';
                        },
                        pointRadius: (context) => {
                            const index = context.dataIndex;
                            if (index === 0) return 4;
                            const change = changes[index-1];
                            return change !== 0 ? 5 : 3;
                        },
                        tension: 0.1,
                        fill: true
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        tooltip: {
                            callbacks: {
                                label: (context) => {
                                    let label = context.dataset.label || '';
                                    if (label) label += ': ';
                                    label += new Intl.NumberFormat('ru-RU', {
                                        style: 'currency',
                                        currency: 'RUB',
                                        maximumFractionDigits: 0
                                    }).format(context.raw).replace('RUB', '₽');
                                    
                                    if (context.dataIndex > 0) {
                                        const change = changes[context.dataIndex-1];
                                        if (change !== 0) {
                                            const absChange = Math.abs(change);
                                            const formattedChange = new Intl.NumberFormat('ru-RU', {
                                                style: 'currency',
                                                currency: 'RUB',
                                                maximumFractionDigits: 0
                                            }).format(absChange).replace('RUB', '₽');
                                            label += ` (${change > 0 ? '+' : ''}${formattedChange})`;
                                        }
                                    }
                                    return label;
                                }
                            }
                        },
                        legend: {
                            display: false
                        }
                    },
                    scales: {
                        x: {
                            grid: {
                                display: false
                            }
                        },
                        y: {
                            beginAtZero: false,
                            ticks: {
                                callback: (value) => new Intl.NumberFormat('ru-RU', {
                                    style: 'currency',
                                    currency: 'RUB',
                                    maximumFractionDigits: 0
                                }).format(value).replace('RUB', '₽')
                            },
                            grid: {
                                color: 'rgba(0, 0, 0, 0.1)'
                            }
                        }
                    }
                }
            });
            
        } catch (error) {
            console.error('Ошибка при построении графика:', error);
            chartContainer.textContent = 'Ошибка при отображении графика';
        }
    }

    function initMap(data) {
        const apartmentAddress = data.buildingInfo.address;
        
        if (!apartmentAddress) {
            showNotification('Карта недоступна: отсутствуют координаты');
            return;
        }
        
        ymaps.ready(function() {
            ymaps.geocode(apartmentAddress, {
                results: 1
            }).then(function (res) {
                const firstGeoObject = res.geoObjects.get(0);
                
                if (!firstGeoObject) {
                    document.getElementById('map').innerHTML = 'Не удалось определить координаты по адресу';
                    return;
                }
                
                const coordinates = firstGeoObject.geometry.getCoordinates();
                
                const map = new ymaps.Map("map", {
                    center: coordinates,
                    zoom: 15,
                    controls: ['zoomControl', 'typeSelector', 'fullscreenControl']
                });
                
                const apartmentPlacemark = new ymaps.Placemark(coordinates, {
                    hintContent: data.buildingInfo.residentialComplex,
                    balloonContent: data.buildingInfo.address
                }, {
                    preset: 'islands#darkBlueHomeIcon',
                    iconColor: '#40027E'
                });
            
                map.geoObjects.add(apartmentPlacemark);
                
                addInfrastructureMarker(map, data.nearestMetroInfo.nearestMetroCoordinates, 
                    `Метро ${data.nearestMetroInfo.name}`, 'metro', '#FF0000');
                addInfrastructureMarker(map, data.nearestSchoolInfo.nearestSchoolCoordinates, 
                    data.nearestSchoolInfo.name, 'school', '#0000FF');
                addInfrastructureMarker(map, data.nearestKindergartenInfo.nearestKindergartenCoordinates, 
                    data.nearestKindergartenInfo.name, 'kindergarten', '#FFA500');
                addInfrastructureMarker(map, data.nearestShopInfo.nearestShopCoordinates, 
                    `Магазин ${data.nearestShopInfo.name}`, 'shop', '#800080');
                addInfrastructureMarker(map, data.nearestPharmacyInfo.nearestPharmacyCoordinates, 
                    `Аптека ${data.nearestPharmacyInfo.name}`, 'pharmacy', '#008000');
            }).catch(function (error) {
                console.error('Ошибка геокодирования:', error);
                showNotification('Ошибка определения местоположения');
            });
        }); 
    }   

    function addInfrastructureMarker(map, coords, name, type, color) {
        if (!coords) return;
        
        const iconMap = {
            'metro': 'railway',
            'school': 'education',
            'kindergarten': 'education',
            'shop': 'shop',
            'pharmacy': 'pharmacy'
        };
        
        const placemark = new ymaps.Placemark(coords.split(',').map(Number), {
            hintContent: name,
            balloonContent: name
        }, {
            preset: `islands#${iconMap[type] || 'circle'}Icon`,
            iconColor: color
        });
        
        map.geoObjects.add(placemark);
    }

    function setupFavoriteButton() {
        const favoriteBtn = document.querySelector('.favorite-btn');
        const fullscreenFavorite = document.querySelector('.fullscreen-favorite');
        
        if (!favoriteBtn) {
            console.warn('Основная кнопка избранного не найдена');
            return;
        }
        
        favoriteBtn.addEventListener('click', async function(e) {
            e.stopPropagation();
            
            if (!checkAuth()) {
                window.location.href = 'login';
                return;
            }
            
            const icon = this.querySelector('i');
            const isFavorite = icon.classList.contains('fas');
            const fullscreenIcon = fullscreenFavorite ? fullscreenFavorite.querySelector('i') : null;
            
            try {
                let success;
                if (isFavorite) {
                    success = await removeFromFavorites(apartmentId);
                } else {
                    success = await addToFavorites(apartmentId);
                }
                
                if (success) {
                    if (isFavorite) {
                        icon.classList.remove('fas');
                        icon.classList.add('far');
                    } else {
                        icon.classList.remove('far');
                        icon.classList.add('fas');
                    }
                    
                    if (fullscreenIcon) {
                        fullscreenIcon.className = icon.className;
                    }
                }
            } catch (error) {
                console.error('Ошибка обновления избранного:', error);
            }
        });
    }

    function setupMortgageCalculator() {
        const mortgageRates = {
            standard: 0.25, 
            it: 0.05,       
            family: 0.06   
        };
        
        const calculateBtn = document.getElementById('calculate-btn');
        const priceValue = document.getElementById('price-value');
        const percentInput = document.getElementById('initial-payment-percent');
        const sliderInput = document.getElementById('initial-payment-slider');
        const rubValue = document.getElementById('initial-payment-rub');
        
        function syncInputs(value) {
            percentInput.value = value;
            sliderInput.value = value;
            
            sliderInput.style.setProperty('--slider-progress', `${value}%`);
            
            const priceText = priceValue.textContent.trim();
            const price = parseFloat(priceText.replace(/\s/g, '').replace('₽', ''));
            
            if (!isNaN(price)) {
                const initialPayment = Math.round(price * value / 100);
                rubValue.textContent = `${new Intl.NumberFormat('ru-RU').format(initialPayment)} ₽`;
            }
            
            calculateMortgage();
        }
        
        percentInput.addEventListener('input', function() {
            let value = parseInt(this.value);
            if (isNaN(value)) value = 0;
            if (value < 0) value = 0;
            if (value > 100) value = 100;
            syncInputs(value);
        });
        
        sliderInput.addEventListener('input', function() {
            syncInputs(parseInt(this.value));
        });
        
        calculateBtn.addEventListener('click', calculateMortgage);
        
        function calculateMortgage() {
            try {
                const priceText = priceValue.textContent.trim();
                const price = parseFloat(priceText.replace(/\s/g, '').replace('₽', ''));
                
                if (isNaN(price)) {
                    showNotification('Не удалось определить цену квартиры');
                    return;
                }
                
                const mortgageType = document.getElementById('mortgage-type').value;
                const rate = mortgageRates[mortgageType];
                const percent = parseInt(percentInput.value);
                const initialPayment = Math.round(price * percent / 100);
                const loanTermYears = parseInt(document.getElementById('loan-term').value) || 15;
                
                if (loanTermYears < 1 || loanTermYears > 30) {
                    showNotification('Срок кредита должен быть от 1 до 30 лет');
                    return;
                }
                
                const loanAmount = price - initialPayment;
                const monthlyRate = rate / 12;
                const loanTermMonths = loanTermYears * 12;
                
                const monthlyPayment = loanAmount * 
                    (monthlyRate * Math.pow(1 + monthlyRate, loanTermMonths)) / 
                    (Math.pow(1 + monthlyRate, loanTermMonths) - 1);
                
                const totalPayment = monthlyPayment * loanTermMonths;
                const overpayment = totalPayment - loanAmount;
                
                const formatCurrency = (value) => {
                    return new Intl.NumberFormat('ru-RU', {
                        style: 'currency',
                        currency: 'RUB',
                        maximumFractionDigits: 0
                    }).format(value).replace('RUB', '₽');
                };
                
                const formatPercent = (value) => {
                    return (value * 100).toFixed(2) + '%';
                };
                
                document.getElementById('interest-rate').textContent = formatPercent(rate);
                document.getElementById('monthly-payment').textContent = formatCurrency(monthlyPayment);
                document.getElementById('total-payment').textContent = formatCurrency(totalPayment);
                document.getElementById('overpayment').textContent = formatCurrency(overpayment);
                
            } catch (error) {
                showNotification('Ошибка расчета ипотеки: ' + error.message);
            }
        }
        
        sliderInput.style.setProperty('--slider-progress', '15%');
        
        function updateInitialPayment() {
            const priceText = priceValue.textContent.trim();
            const price = parseFloat(priceText.replace(/\s/g, '').replace('₽', ''));
            
            if (!isNaN(price)) {
                const initialPayment = Math.round(price * 15 / 100);
                rubValue.textContent = `${new Intl.NumberFormat('ru-RU').format(initialPayment)} ₽`;
                calculateMortgage();
            }
        }
        
        const priceObserver = new MutationObserver(() => {
            if (priceValue.textContent.trim() !== 'Загрузка...') {
                updateInitialPayment();
                priceObserver.disconnect();
            }
        });
        
        priceObserver.observe(priceValue, { childList: true, characterData: true, subtree: true });
    }

    async function checkFavoriteStatus() {
        if (!checkAuth()) {
            const favoriteBtn = document.querySelector('.favorite-btn');
            if (favoriteBtn) favoriteBtn.style.display = 'none';
            return false;
        }

        try {
            const response = await fetch(`${config.api.baseUrl}/user-preferences/favorites`, {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                }
            });
            
            if (response.ok) {
                const favorites = await response.json();
                const isFavorite = favorites.some(fav => fav.flatId === parseInt(apartmentId));
                
                
                const mainIcon = document.querySelector('.favorite-btn i');
                const fullscreenIcon = document.querySelector('.fullscreen-favorite i');
                
                if (mainIcon) {
                    mainIcon.className = isFavorite ? 'fas fa-heart' : 'far fa-heart';
                }
                
                if (fullscreenIcon) {
                    fullscreenIcon.className = isFavorite ? 'fas fa-heart' : 'far fa-heart';
                }
                
                return isFavorite;
            }
            return false;
        } catch (error) {
            console.error('Ошибка проверки избранного:', error);
            return false;
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
    
    checkFavoriteStatus().then(isFavorite => {
        const mainFavoriteIcon = document.querySelector('.favorite-btn i');
        const fullscreenFavoriteIcon = document.querySelector('.fullscreen-favorite i');
        
        if (mainFavoriteIcon && fullscreenFavoriteIcon) {
            if (isFavorite) {
                mainFavoriteIcon.classList.remove('far');
                mainFavoriteIcon.classList.add('fas');
                fullscreenFavoriteIcon.classList.remove('far');
                fullscreenFavoriteIcon.classList.add('fas');
            } else {
                mainFavoriteIcon.classList.remove('fas');
                mainFavoriteIcon.classList.add('far');
                fullscreenFavoriteIcon.classList.remove('fas');
                fullscreenFavoriteIcon.classList.add('far');
            }
        }
    });

    loadApartmentData().then(() => {
        setupFavoriteButton();
        checkFavoriteStatus();
    });
    setupMortgageCalculator();
});