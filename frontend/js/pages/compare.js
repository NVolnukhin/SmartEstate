import { initTokenChecker, checkTokenExpiration } from '../components/auth/token_сhecker.js';
import { config } from '../config.js';

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
    
    const urlParams = new URLSearchParams(window.location.search);
    const flat1Id = urlParams.get('id1');
    const flat2Id = urlParams.get('id2');
    
    if (!flat1Id || !flat2Id) {
        window.location.href = 'all_flats';
        return;
    }
    
    const loadingIndicator = document.getElementById('loading-indicator');
    let flat1Data = null;
    let flat2Data = null;
    
    async function loadComparisonData() {
        try {
            loadingIndicator.style.display = 'block';
            
            const [response1, response2] = await Promise.all([
                fetch(`${config.api.baseUrl}/flats/${flat1Id}`),
                fetch(`${config.api.baseUrl}/flats/${flat2Id}`)
            ]);
            
            if (!response1.ok || !response2.ok) {
                throw new Error('Ошибка загрузки данных');
            }
            
            flat1Data = await response1.json();
            flat2Data = await response2.json();
            
            renderComparison();
            
            if (checkAuth()) {
                saveComparisonHistory();
            }
            
        } catch (error) {
            console.error('Ошибка загрузки данных:', error);
            document.getElementById('comparisonDetails').innerHTML = `
                <div style="text-align: center; color: #40027E; padding: 30px;">
                    Не удалось загрузить данные для сравнения. Пожалуйста, попробуйте позже.
                </div>
            `;
        } finally {
            loadingIndicator.style.display = 'none';
        }
    }
    
    function renderComparison() {
        if (!flat1Data || !flat2Data) return;
        
        renderFlatsPhotos();
        renderComparisonDetails();
        highlightBestValues();
        setupFavoriteButtons();
    }
    
    function renderFlatsPhotos() {
        const container = document.getElementById('flatsComparison');
        container.innerHTML = '';
        
        const formatPrice = (price) => {
            return new Intl.NumberFormat('ru-RU', {
                style: 'currency',
                currency: 'RUB',
                maximumFractionDigits: 0
            }).format(price).replace('RUB', '₽');
        };
        
        const mainImage1 = flat1Data.images && flat1Data.images.length > 0 ? 
            flat1Data.images[0] : 
            'https://via.placeholder.com/600x400/40027E/FFE4AA?text=No+Image';
        
        const mainImage2 = flat2Data.images && flat2Data.images.length > 0 ? 
            flat2Data.images[0] : 
            'https://via.placeholder.com/600x400/40027E/FFE4AA?text=No+Image';
        
        container.innerHTML = `
            <div class="flat-photo">
                <img src="${mainImage1}" alt="Квартира 1">
                <button class="favorite-btn" data-id="${flat1Id}">
                    <i class="far fa-heart"></i>
                </button>
                <div style="position: absolute; bottom: 15px; left: 15px; right: 15px; text-align: center;">
                    <div style="background: rgba(255, 255, 255, 0.9); padding: 8px; border-radius: 6px; color: #40027E; font-weight: 700;">
                        ${formatPrice(flat1Data.price)}
                    </div>
                    <a href="flat?id=${flat1Id}" class="view-details-btn">
                        <i class="fas fa-info-circle"></i> Подробнее
                    </a>
                </div>
            </div>
            <div class="flat-photo">
                <img src="${mainImage2}" alt="Квартира 2">
                <button class="favorite-btn" data-id="${flat2Id}">
                    <i class="far fa-heart"></i>
                </button>
                <div style="position: absolute; bottom: 15px; left: 15px; right: 15px; text-align: center;">
                    <div style="background: rgba(255, 255, 255, 0.9); padding: 8px; border-radius: 6px; color: #40027E; font-weight: 700;">
                        ${formatPrice(flat2Data.price)}
                    </div>
                    <a href="flat?id=${flat2Id}" class="view-details-btn">
                        <i class="fas fa-info-circle"></i> Подробнее
                    </a>
                </div>
            </div>
        `;
    }
    
    function renderComparisonDetails() {
        const container = document.getElementById('comparisonDetails');
        
        const renderPriceWithTooltip = (flatData) => {
            if (!flatData.priceChart || flatData.priceChart.length <= 1) {
                return new Intl.NumberFormat('ru-RU', {
                    style: 'currency',
                    currency: 'RUB',
                    maximumFractionDigits: 0
                }).format(flatData.price).replace('RUB', '₽');
            }
            
            const sortedHistory = [...flatData.priceChart]
                .map(item => ({
                    date: new Date(item.date),
                    price: Number(item.price)
                }))
                .sort((a, b) => b.date - a.date)
                .slice(0, 3);
            
            let tooltipContent = `
                <table class="price-change-table">
                    <thead>
                        <tr>
                            <th style="width: 30%;">Дата</th>
                            <th style="width: 35%;">Цена</th>
                            <th style="width: 35%;">Изменение</th>
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
                            <span class="${changeClass}">
                                ${arrow} ${formattedChange}
                            </span>`;
                    } else {
                        changeHtml = '<span style="color: #999;">—</span>';
                    }
                } else {
                    changeHtml = '<span style="color: #999;">—</span>';
                }
                
                tooltipContent += `
                    <tr>
                        <td>${date}</td>
                        <td>${formattedPrice}</td>
                        <td>${changeHtml}</td>
                    </tr>`;
            }
            
            tooltipContent += `</tbody></table>`;
            
            return `
                ${new Intl.NumberFormat('ru-RU', {
                    style: 'currency',
                    currency: 'RUB',
                    maximumFractionDigits: 0
                }).format(flatData.price).replace('RUB', '₽')}
                <span class="price-hint">
                    <i class="fas fa-history"></i>
                    <span class="tooltip">
                        <div style="font-weight: 700; margin-bottom: 8px;">История изменения цены</div>
                        ${tooltipContent}
                    </span>
                </span>
            `;
        };
        
        container.innerHTML = `
            <h2 class="section-title">Основные характеристики</h2>
            
            <div class="comparison-grid">
                <div class="comparison-label">Цена</div>
                <div class="comparison-value">${renderPriceWithTooltip(flat1Data)}</div>
                <div class="comparison-value">${renderPriceWithTooltip(flat2Data)}</div>
                
                <div class="comparison-label">Площадь</div>
                <div class="comparison-value">${flat1Data.square} м²</div>
                <div class="comparison-value">${flat2Data.square} м²</div>
                
                <div class="comparison-label">Комнатность</div>
                <div class="comparison-value">${flat1Data.roominess === -1 ? 'Студия' : `${flat1Data.roominess}-комнатная`}</div>
                <div class="comparison-value">${flat2Data.roominess === -1 ? 'Студия' : `${flat2Data.roominess}-комнатная`}</div>
                
                <div class="comparison-label">Этаж</div>
                <div class="comparison-value">${flat1Data.floor} / ${flat1Data.buildingInfo.floorCount}</div>
                <div class="comparison-value">${flat2Data.floor} / ${flat2Data.buildingInfo.floorCount}</div>
                
                <div class="comparison-label">Тип отделки</div>
                <div class="comparison-value">${flat1Data.finishType}</div>
                <div class="comparison-value">${flat2Data.finishType}</div>
            </div>

            <h3 class="section-title">Информация о здании</h3>
            <div class="comparison-grid">
                <div class="comparison-label">Статус здания</div>
                <div class="comparison-value">${flat1Data.buildingInfo.status}</div>
                <div class="comparison-value">${flat2Data.buildingInfo.status}</div>
                
                <div class="comparison-label">ЖК</div>
                <div class="comparison-value">${flat1Data.buildingInfo.residentialComplex}</div>
                <div class="comparison-value">${flat2Data.buildingInfo.residentialComplex}</div>
                
                <div class="comparison-label">Адрес</div>
                <div class="comparison-value">${flat1Data.buildingInfo.address}</div>
                <div class="comparison-value">${flat2Data.buildingInfo.address}</div>
            </div>

            <h3 class="section-title">Инфраструктура</h3>
            <div class="comparison-grid">    
                <div class="comparison-label">До ближайшего метро</div>
                <div class="comparison-value">
                    ${flat1Data.nearestMetroInfo.name} (${flat1Data.nearestMetroInfo.minutesToMetro} мин)
                </div>
                <div class="comparison-value">
                    ${flat2Data.nearestMetroInfo.name} (${flat2Data.nearestMetroInfo.minutesToMetro} мин)
                </div>
                
                <div class="comparison-label">До школы</div>
                <div class="comparison-value">${flat1Data.nearestSchoolInfo.name} (${flat1Data.nearestSchoolInfo.minutesToSchool} мин)</div>
                <div class="comparison-value">${flat2Data.nearestSchoolInfo.name} (${flat2Data.nearestSchoolInfo.minutesToSchool} мин)</div>
                
                <div class="comparison-label">До детского сада</div>
                <div class="comparison-value">${flat1Data.nearestKindergartenInfo.name} (${flat1Data.nearestKindergartenInfo.minutesToKindergarten} мин)</div>
                <div class="comparison-value">${flat2Data.nearestKindergartenInfo.name} (${flat2Data.nearestKindergartenInfo.minutesToKindergarten} мин)</div>
                
                <div class="comparison-label">До магазина</div>
                <div class="comparison-value">${flat1Data.nearestShopInfo.name} (${flat1Data.nearestShopInfo.minutesToShop} мин)</div>
                <div class="comparison-value">${flat2Data.nearestShopInfo.name} (${flat2Data.nearestShopInfo.minutesToShop} мин)</div>
                
                <div class="comparison-label">До аптеки</div>
                <div class="comparison-value">${flat1Data.nearestPharmacyInfo.name} (${flat1Data.nearestPharmacyInfo.minutesToPharmacy} мин)</div>
                <div class="comparison-value">${flat2Data.nearestPharmacyInfo.name} (${flat2Data.nearestPharmacyInfo.minutesToPharmacy} мин)</div>
            </div>

            <h3 class="section-title">Информация о застройщике</h3>
            <div class="comparison-grid">
                <div class="comparison-label">Имя</div>
                <div class="comparison-value">${flat1Data.developerInfo.name}</div>
                <div class="comparison-value">${flat2Data.developerInfo.name}</div>
                
                <div class="comparison-label">Построенных зданий</div>
                <div class="comparison-value">${flat1Data.developerInfo.buildingsCount}</div>
                <div class="comparison-value">${flat2Data.developerInfo.buildingsCount}</div>
                
                <div class="comparison-label">Веб-сайт</div>
                <div class="comparison-value">
                    <a href="${flat1Data.developerInfo.website}" target="_blank">${flat1Data.developerInfo.website}</a>
                </div>
                <div class="comparison-value">
                    <a href="${flat2Data.developerInfo.website}" target="_blank">${flat2Data.developerInfo.website}</a>
                </div>
            </div>
        `;
    }
    
    function highlightBestValues() {
        highlightSection({
            selector: '.comparison-grid:nth-of-type(1)',
            rules: [
                { index: 0, type: 'min', selector: '.comparison-value' }, 
                { index: 1, type: 'max', selector: '.comparison-value' }, 
                { index: 2, type: 'max', selector: '.comparison-value' }  
            ]
        });
    
        highlightSection({
            selector: '.comparison-grid:nth-of-type(3)',
            rules: [
                { index: 0, type: 'min-time', selector: '.comparison-value' }, 
                { index: 1, type: 'min-time', selector: '.comparison-value' }, 
                { index: 2, type: 'min-time', selector: '.comparison-value' }, 
                { index: 3, type: 'min-time', selector: '.comparison-value' }, 
                { index: 4, type: 'min-time', selector: '.comparison-value' }  
            ]
        });
    
        highlightSection({
            selector: '.comparison-grid:nth-of-type(4)',
            rules: [
                { index: 1, type: 'max', selector: '.comparison-value' } 
            ]
        });
    
        function highlightSection({selector, rules}) {
            const section = document.querySelector(selector);
            if (!section) return;
    
            const labels = section.querySelectorAll('.comparison-label');
            
            rules.forEach(rule => {
                if (rule.index >= labels.length) return;
                
                const label = labels[rule.index];
                const values = [];
                let next = label.nextElementSibling;
                
                while (next && next.matches(rule.selector)) {
                    values.push(next);
                    next = next.nextElementSibling;
                }
            
                if (values.length < 2) return;
                
                const extracted = values.map(el => {
                    switch (rule.type) {
                        case 'min-time':
                            return extractTimeValue(el.textContent);
                        case 'roominess':
                            return extractRoominess(el.textContent);
                        case 'buildings':
                            return extractBuildingsCount(el.textContent);
                        case 'min':
                        case 'max':
                        default:
                            return extractValue(el.textContent);
                    }
                });
                
                if (extracted.some(val => val === null)) return;
                
                const allEqual = extracted.every(val => val === extracted[0]);
                if (allEqual) return; 

                let bestIndex;
                if (rule.type.includes('min')) {
                    bestIndex = extracted.indexOf(Math.min(...extracted));
                } else {
                    bestIndex = extracted.indexOf(Math.max(...extracted));
                }
                
                if (bestIndex >= 0) {
                    values[bestIndex].classList.add('highlight');
                }
            });
        }
    
        function extractTimeValue(text) {
            const match = text.match(/\((\d+)\s*мин\)/);
            return match ? parseInt(match[1]) : null;
        }
    
        function extractRoominess(text) {
            if (typeof text !== 'string') {
                return null;
            }
        
            const normalizedText = text.trim().toLowerCase();
        
            if (normalizedText.includes('студия')) {
                return 0;
            }
        
            const match = normalizedText.match(/^(\d+)-комнатная$/);
            if (match) {
                return parseInt(match[1], 10);
            }
        
            return null;
        }
    
        function extractBuildingsCount(text) {
            const match = text.match(/(\d+)\s*домов/);
            return match ? parseInt(match[1]) : null;
        }
    
        function extractValue(text) {
            if (text.includes("₽")) {
                const price = text.replace(/[^\d]/g, '');
                return price ? parseInt(price) : null;
            }
            
            if (text.includes("м²")) {
                const match = text.match(/(\d+(\.\d+)?)\s*м²/);
                return match ? parseFloat(match[1]) : null;
            }
            
            const num = parseFloat(text.replace(/\s/g, ''));
            return isNaN(num) ? null : num;
        }
    }

    async function saveComparisonHistory() {
        try {
            const response = await fetch(`${config.api.baseUrl}/user-preferences/comparisons`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                },
                body: JSON.stringify({
                    flatId1: parseInt(flat1Id),
                    flatId2: parseInt(flat2Id)
                })
            });
            
            if (!response.ok) {
                console.error('Ошибка сохранения истории сравнения');
            }
        } catch (error) {
            console.error('Ошибка сохранения истории сравнения:', error);
        }
    }
    
    function setupFavoriteButtons() {
        document.querySelectorAll('.favorite-btn').forEach(btn => {
            btn.addEventListener('click', async function(e) {
                e.stopPropagation();
                
                if (!checkAuth()) {
                    window.location.href = 'login';
                    return;
                }
                
                const flatId = this.getAttribute('data-id');
                const icon = this.querySelector('i');
                
                try {
                    let success;
                    if (icon.classList.contains('fas')) {
                        success = await removeFromFavorites(flatId);
                    } else {
                        success = await addToFavorites(flatId);
                    }
                    
                    if (success) {
                        icon.classList.toggle('fas');
                        icon.classList.toggle('far');
                    }
                } catch (error) {
                    console.error('Ошибка обновления избранного:', error);
                }
            });
        });
        
        checkFavoriteStatus();
    }
    
    async function checkFavoriteStatus() {
        if (!checkAuth()) return;
        
        try {
            const response = await fetch(`${config.api.baseUrl}/user-preferences/favorites`, {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                }
            });
            
            if (response.ok) {
                const favorites = await response.json();
                
                document.querySelectorAll('.favorite-btn').forEach(btn => {
                    const flatId = btn.getAttribute('data-id');
                    const icon = btn.querySelector('i');
                    
                    const isFavorite = favorites.some(fav => fav.flatId == flatId);
                    if (isFavorite) {
                        icon.classList.remove('far');
                        icon.classList.add('fas');
                    } else {
                        icon.classList.remove('fas');
                        icon.classList.add('far');
                    }
                });
            }
        } catch (error) {
            console.error('Ошибка проверки избранного:', error);
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
    
    loadComparisonData();
});