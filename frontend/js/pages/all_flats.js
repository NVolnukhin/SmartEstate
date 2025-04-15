import { showNotification, initNotification } from '../components/ui/notification.js';
import { initTokenChecker, checkTokenExpiration } from '../components/auth/token_сhecker.js';
import { config } from '../config.js';

document.addEventListener('DOMContentLoaded', function() {
    initNotification();
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
    let currentPage = parseInt(urlParams.get('page')) || 
                        parseInt(localStorage.getItem('lastPage')) || 1;
    let totalPages = 1;
    let allFlatsData = [];
    
    let comparisonList = JSON.parse(localStorage.getItem('comparisonList')) || [];
    const comparisonToggle = document.querySelector('.dropdown-toggle');
    const comparisonSidebar = document.querySelector('.comparison-sidebar');
    const comparisonItems = document.querySelector('.comparison-items');
    const compareBtn = document.querySelector('.compare-btn');
    const clearComparisonBtn = document.querySelector('.clear-comparison');
    const loadingIndicator = document.getElementById('loading-indicator');
    
    const filtersToggle = document.getElementById('filtersToggle');
    const filtersSidebar = document.getElementById('filtersSidebar');
    const applyFiltersBtn = document.querySelector('.apply-filters');
    const resetFiltersBtn = document.querySelector('.reset-filters');
    
    async function loadMetroStations() {
        try {
            const response = await fetch(`${config.api.baseUrl}/metro`);
            if (!response.ok) throw new Error(`Ошибка HTTP: ${response.status}`);
            
            const stations = await response.json();
            const metroSelect = $('#metroSelect');
            
            metroSelect.empty();
            stations.forEach(station => {
                metroSelect.append(new Option(station.name, station.id));
            });
            
            metroSelect.select2({
                placeholder: "Выберите станции метро...",
                allowClear: true,
                width: '100%',
                dropdownParent: $(filtersSidebar),
                closeOnSelect: true
            })
            .on('select2:select', () => metroSelect.select2('close'))
            .on('select2:unselect', e => e.params.originalEvent.stopPropagation());
            
            const savedFilters = JSON.parse(localStorage.getItem('flatFilters'));
            if (savedFilters?.metroStations?.length) {
                metroSelect.val(savedFilters.metroStations).trigger('change');
            }
            
        } catch (error) {
            console.error('Ошибка загрузки станций:', error);
            showNotification('Ошибка загрузки станций метро');
        }
    }

    async function loadDevelopers() {
        try {
            const response = await fetch(`${config.api.baseUrl}/developer`);
            if (!response.ok) throw new Error(`Ошибка HTTP: ${response.status}`);
            
            const developers = await response.json();
            const developerSelect = $('#developerSelect');
            
            // Полностью очищаем select перед заполнением
            developerSelect.empty();
            
            developers.forEach(developer => {
                if (developer.name && developer.developerId !== undefined) {
                    developerSelect.append(new Option(developer.name, developer.developerId));
                }
            });
            
            developerSelect.select2({
                placeholder: "Выберите застройщиков...",
                allowClear: true,
                width: '100%',
                dropdownParent: $(filtersSidebar),
                closeOnSelect: true,
                // minimumResultsForSearch: 1
            }).on('select2:select', function() {
                $(this).select2('close');
            }).on('select2:unselect', function(e) {
                e.params.originalEvent.stopPropagation();
            });
            
            const savedFilters = JSON.parse(localStorage.getItem('flatFilters')) || {};
            if (savedFilters.developers && savedFilters.developers.length > 0) {
                developerSelect.val(savedFilters.developers).trigger('change');
            }
            
        } catch (error) {
            console.error('Ошибка загрузки застройщиков:', error);
            showNotification('Ошибка загрузки списка застройщиков');
        }
    }

    filtersToggle.addEventListener('click', function() {
        this.classList.toggle('active');
        filtersSidebar.classList.toggle('active');
    });
    
    async function loadFlats(page = 1) {
        renderComparisonList();
        loadingIndicator.style.display = 'block';
        currentPage = page;
        localStorage.setItem('lastPage', page.toString());
        
        window.history.pushState({}, '', `?page=${page}`);
        
        try {
            const savedFilters = JSON.parse(localStorage.getItem('flatFilters')) || {};
            
            const params = new URLSearchParams();
            params.append('page', page);
            
            if (savedFilters.minPrice) params.append('minPrice', savedFilters.minPrice);
            if (savedFilters.maxPrice) params.append('maxPrice', savedFilters.maxPrice);
            if (savedFilters.minSquare) params.append('minSquare', savedFilters.minSquare);
            if (savedFilters.maxSquare) params.append('maxSquare', savedFilters.maxSquare);
            if (savedFilters.roominess?.length > 0) params.append('roominess', savedFilters.roominess.join(','));
            if (savedFilters.metroStations?.length > 0) params.append('metroStations', savedFilters.metroStations.join(','));
            if (savedFilters.metroTime && savedFilters.metroTime !== '0') params.append('maxMetroTime', savedFilters.metroTime);
            if (savedFilters.developers?.length > 0) params.append('developers', savedFilters.developers.join(','));
            if (savedFilters.minFloor) params.append('minFloor', savedFilters.minFloor);
            if (savedFilters.maxFloor) params.append('maxFloor', savedFilters.maxFloor);
            if (savedFilters.minFloorCount) params.append('minFloorCount', savedFilters.minFloorCount);
            if (savedFilters.maxFloorCount) params.append('maxFloorCount', savedFilters.maxFloorCount);
            if (savedFilters.buildingStatus?.length > 0) params.append('buildingStatus', savedFilters.buildingStatus.join(','));
            
            const response = await fetch(`${config.api.baseUrl}/flats?${params.toString()}`);
            const data = await response.json();
            
            if (response.ok) {
                allFlatsData = data.items;
                renderFlats(data.items);
                updatePagination(data.totalPages, page);
                renderComparisonList();
            } else {
                console.error('Ошибка загрузки квартир:', data.message);
                renderError();
            }
        } catch (error) {
            console.error('Ошибка соединения:', error);
            renderError();
        } finally {
            loadingIndicator.style.display = 'none';
        }
    }
    
    function renderFlats(flats) {
        const container = document.getElementById('flatsList');
        container.innerHTML = '';

        localStorage.setItem('allFlatsData', JSON.stringify(flats));
        
        flats.forEach(flat => {
            const card = document.createElement('div');
            card.className = 'flat-card';
            card.setAttribute('data-id', flat.flatId);
            
            const formattedPrice = new Intl.NumberFormat('ru-RU', {
                style: 'currency',
                currency: 'RUB',
                maximumFractionDigits: 0
            }).format(flat.price).replace('RUB', '₽');
            
            const mainImage = flat.images && flat.images.length > 0 ? 
                flat.images[0] : 
                'https://via.placeholder.com/240x220/40027E/FFE4AA?text=No+Image';
            
            card.innerHTML = `
                <div class="flat-image">
                    <img src="${mainImage}" alt="ЖК ${flat.building.residentialComplex}">
                </div>
                <div class="flat-info">
                    <div class="flat-price">${formattedPrice}</div>
                    <div class="flat-details">
                        <div class="detail-item">
                            <div class="detail-label">Площадь</div>
                            <div>${flat.square} м²</div>
                        </div>
                        <div class="detail-item">
                            <div class="detail-label">Комнатность</div>
                            <div>${flat.roominess === -1 ? 'Студия' : `${flat.roominess}-комнатная`}</div>
                        </div>
                        <div class="detail-item">
                            <div class="detail-label">Этаж</div>
                            <div>${flat.floor}/${flat.building.floorCount}</div>
                        </div>
                        <div class="detail-item">
                            <div class="detail-label">Метро</div>
                            <div>
                                <i class="fas fa-subway metro-icon"></i>
                                ${flat.nearestMetro.name} (${flat.nearestMetro.minutesToMetro} мин)
                            </div>
                        </div>
                        <div class="detail-item">
                            <div class="detail-label">ЖК</div>
                            <div>${flat.building.residentialComplex}</div>
                        </div>
                        <div class="detail-item">
                            <div class="detail-label">Статус</div>
                            <div>${flat.building.status}</div>
                        </div>
                    </div>
                </div>
                <div class="flat-actions">
                    <button class="action-btn favorite-btn" data-id="${flat.flatId}">
                        <i class="far fa-heart"></i> Избранное
                    </button>
                    <button class="action-btn compare-btn" data-id="${flat.flatId}">
                        <i class="fas fa-balance-scale"></i> Сравнить
                    </button>
                </div>
            `;
            
            container.appendChild(card);
        });
        
        initFavoriteButtons();
        initializeComparisonButtons();
        
        document.querySelectorAll('.flat-card').forEach(card => {
            card.addEventListener('click', function(e) {
                if (e.target.closest('.action-btn')) {
                    return;
                }
                
                const flatId = this.getAttribute('data-id');
                localStorage.setItem('lastPage', currentPage.toString());
                goToFlatPage(flatId);
            });
        });
    }
    
    function updatePagination(totalPagesCount, currentPage) {
        totalPages = totalPagesCount;
        const pagination = document.getElementById('pagination');
        pagination.innerHTML = '';
        
        const prevBtn = document.createElement('button');
        prevBtn.className = 'pagination-btn';
        prevBtn.innerHTML = '<i class="fas fa-chevron-left"></i>';
        prevBtn.disabled = currentPage === 1;
        prevBtn.addEventListener('click', () => {
            if (currentPage > 1) {
                loadFlats(currentPage - 1);
            }
        });
        pagination.appendChild(prevBtn);
        
        const startPage = Math.max(1, currentPage - 2);
        const endPage = Math.min(totalPages, currentPage + 2);
        
        for (let i = startPage; i <= endPage; i++) {
            const pageBtn = document.createElement('button');
            pageBtn.className = `pagination-btn ${i === currentPage ? 'active' : ''}`;
            pageBtn.textContent = i;
            pageBtn.addEventListener('click', () => {
                if (i !== currentPage) {
                    loadFlats(i);
                }
            });
            pagination.appendChild(pageBtn);
        }
        
        const nextBtn = document.createElement('button');
        nextBtn.className = 'pagination-btn';
        nextBtn.innerHTML = '<i class="fas fa-chevron-right"></i>';
        nextBtn.disabled = currentPage === totalPages;
        nextBtn.addEventListener('click', () => {
            if (currentPage < totalPages) {
                loadFlats(currentPage + 1);
            }
        });
        pagination.appendChild(nextBtn);
    }
    
    function renderError() {
        const container = document.getElementById('flatsList');
        container.innerHTML = `
            <div style="color: #FFE4AA; text-align: center; width: 100%; padding: 20px;">
                Не удалось загрузить список квартир. Пожалуйста, попробуйте позже.
            </div>
        `;
    }
    
    function toggleSidebar(forceOpen = false) {
        if (forceOpen || comparisonList.length > 0) {
            comparisonToggle.classList.add('active');
            comparisonSidebar.classList.add('active');
        } else {
            comparisonToggle.classList.remove('active');
            comparisonSidebar.classList.remove('active');
        }
        updateEmptyMessage();
    }            

    comparisonToggle.addEventListener('click', function() {
        this.classList.toggle('active');
        comparisonSidebar.classList.toggle('active');
        updateEmptyMessage();
    });            
    

    function updateEmptyMessage() {
        const emptyMessage = document.querySelector('.empty-comparison');
        emptyMessage.style.display = comparisonList.length === 0 ? 'block' : 'none';
    }

    function saveComparisonList() {
        localStorage.setItem('comparisonList', JSON.stringify(comparisonList));
    }

    function updateCompareButton() {
        compareBtn.style.display = comparisonList.length === 2 ? 'block' : 'none';
        clearComparisonBtn.style.display = comparisonList.length > 0 ? 'block' : 'none';
    }

    function renderComparisonList() {
        comparisonItems.innerHTML = '';
        
        comparisonList.forEach(flat => {
            const formattedPrice = new Intl.NumberFormat('ru-RU', {
                style: 'currency',
                currency: 'RUB',
                maximumFractionDigits: 0
            }).format(flat.price).replace('RUB', '₽');
            
            const mainImage = flat.images && flat.images.length > 0 ? 
                flat.images[0] : 
                'https://via.placeholder.com/240x220/40027E/FFE4AA?text=No+Image';
            
            const comparisonCard = document.createElement('div');
            comparisonCard.className = 'comparison-card';
            comparisonCard.setAttribute('data-id', flat.flatId);
            comparisonCard.innerHTML = `
                <button class="remove-comparison"><i class="fas fa-times"></i></button>
                <div class="comparison-image">
                    <img src="${mainImage}" alt="Квартира">
                </div>
                <div class="comparison-price">${formattedPrice}</div>
                <div class="comparison-details">
                    <div class="comparison-detail">
                        <div class="comparison-detail-label">Комнатность</div>
                        <div>${flat.roominess === -1 ? 'Студия' : `${flat.roominess}-комнатная`}</div>
                    </div>
                    <div class="comparison-detail">
                        <div class="comparison-detail-label">Площадь</div>
                        <div>${flat.square} м²</div>
                    </div>
                    <div class="comparison-detail">
                        <div class="comparison-detail-label">Этаж</div>
                        <div>${flat.floor}/${flat.building.floorCount}</div>
                    </div>
                    <div class="comparison-detail">
                        <div class="comparison-detail-label">Метро</div>
                        <div><i class="fas fa-subway metro-icon"></i> ${flat.nearestMetro.name} (${flat.nearestMetro.minutesToMetro} мин)</div>
                    </div>
                </div>
            `;
            
            comparisonItems.appendChild(comparisonCard);
            
            comparisonCard.querySelector('.remove-comparison').addEventListener('click', function(e) {
                e.stopPropagation();
                removeFromComparison(flat.flatId);
            });
        });
        
        updateCompareButton();
        updateEmptyMessage();
        toggleSidebar();
    }

    function removeFromComparison(flatId) {
        const index = comparisonList.findIndex(item => item.flatId == flatId);
        if (index > -1) {
            comparisonList.splice(index, 1);
            saveComparisonList();
            
            const btn = document.querySelector(`.flat-card[data-id="${flatId}"] .compare-btn`);
            if (btn) {
                btn.classList.remove('active');
                btn.innerHTML = '<i class="fas fa-balance-scale"></i> Сравнить';
            }
            
            renderComparisonList();
            if (comparisonList.length === 0) toggleSidebar(false);
        }
    }
            

    function addToComparison(flatId) {
        if (comparisonList.some(item => item.flatId == flatId)) return false;
        
        if (comparisonList.length >= 2) {
            showNotification("Можно добавить только 2 квартиры для сравнения");
            return false;
        }
        
        const flatToAdd = allFlatsData.find(flat => flat.flatId == flatId);
        if (!flatToAdd) return false;
        
        comparisonList.push(flatToAdd);
        saveComparisonList();
        
        const btn = document.querySelector(`.flat-card[data-id="${flatId}"] .compare-btn`);
        if (btn) {
            btn.classList.add('active');
            btn.innerHTML = '<i class="fas fa-check"></i> В сравнении';
        }
        
        renderComparisonList();
        toggleSidebar(true);
        return true;
    }
    

    function clearComparison() {
        document.querySelectorAll('.compare-btn').forEach(btn => {
            btn.classList.remove('active');
            btn.innerHTML = '<i class="fas fa-balance-scale"></i> Сравнить';
        });
        
        comparisonList = [];
        saveComparisonList();
        renderComparisonList();
    }

    function initializeComparisonButtons() {
        document.querySelectorAll('.compare-btn').forEach(btn => {
            const flatId = btn.getAttribute('data-id');
            if (comparisonList.some(item => item.flatId == flatId)) {
                btn.classList.add('active');
                btn.innerHTML = '<i class="fas fa-check"></i> В сравнении';
            }
            
            btn.addEventListener('click', function(e) {
                e.stopPropagation();
                const flatId = this.getAttribute('data-id');
                
                if (this.classList.contains('active')) {
                    removeFromComparison(flatId);
                } else {
                    addToComparison(flatId);
                }
            });
        });
    }            
    
    function initFavoriteButtons() {
        if (!checkAuth()) 
            return;

        loadFavorites().then(favorites => {
            document.querySelectorAll('.favorite-btn').forEach(btn => {
                const flatId = btn.getAttribute('data-id');
                const icon = btn.querySelector('i');
                
                if (favorites.includes(parseInt(flatId))) {
                    btn.classList.add('active');
                    icon.classList.remove('far');
                    icon.classList.add('fas');
                }
                
                btn.addEventListener('click', async function(e) {
                    e.preventDefault();
                    e.stopPropagation();
                    
                    if (!checkAuth()) {
                        window.location.href = 'login';
                        return;
                    }
                    
                    const flatId = this.getAttribute('data-id');
                    const icon = this.querySelector('i');
                    
                    if (this.classList.contains('active')) {
                        const success = await removeFromFavorites(flatId);
                        if (success) {
                            this.classList.remove('active');
                            icon.classList.remove('fas');
                            icon.classList.add('far');
                        }
                    } else {
                        const success = await addToFavorites(flatId);
                        if (success) {
                            this.classList.add('active');
                            icon.classList.remove('far');
                            icon.classList.add('fas');
                        }
                    }
                });
            });
        });
    }
    
    async function loadFavorites() {
        try {
            const response = await fetch(`${config.api.baseUrl}/user-preferences/favorites`, {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                }
            });
            
            if (response.ok) {
                const data = await response.json();
                return data.map(fav => fav.flatId) || [];
            }
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

    function goToFlatPage(flatId) {
        window.location.href = `flat?id=${flatId}`;
    }

    compareBtn.addEventListener('click', function() {
        if (comparisonList.length === 2) {
            window.location.href = `compare?id1=${comparisonList[0].flatId}&id2=${comparisonList[1].flatId}`;
        }
    });

    clearComparisonBtn.addEventListener('click', clearComparison);
    
    applyFiltersBtn.addEventListener('click', async function() {
        const filters = {
            minPrice: document.getElementById('minPrice').value,
            maxPrice: document.getElementById('maxPrice').value,
            minSquare: document.getElementById('minSquare').value,
            maxSquare: document.getElementById('maxSquare').value,
            roominess: Array.from(document.querySelectorAll('.checkbox-input[name="roominess"]:checked')).map(el => el.value),
            metroStations: $('#metroSelect').val() || [],
            metroTime: document.querySelector('input[name="metroTime"]:checked').value,
            developers: $('#developerSelect').val() || [],
            minFloor: document.getElementById('minFloor').value,
            maxFloor: document.getElementById('maxFloor').value,
            minFloorCount: document.getElementById('minFloorCount').value,
            maxFloorCount: document.getElementById('maxFloorCount').value,
            buildingStatus: Array.from(document.querySelectorAll('.checkbox-input[name="buildingStatus"]:checked')).map(el => el.value)
        };
        
        localStorage.setItem('flatFilters', JSON.stringify(filters));
        
        loadingIndicator.style.display = 'block';
        
        try {
            const params = new URLSearchParams();
            
            if (filters.minPrice) params.append('minPrice', filters.minPrice);
            if (filters.maxPrice) params.append('maxPrice', filters.maxPrice);
            if (filters.minSquare) params.append('minSquare', filters.minSquare);
            if (filters.maxSquare) params.append('maxSquare', filters.maxSquare);
            if (filters.roominess.length > 0) params.append('roominess', filters.roominess.join(','));
            if (filters.metroStations.length > 0) {
                params.append('metroStations', filters.metroStations.join(','));
            }
            if (filters.metroTime && filters.metroTime !== '0') params.append('maxMetroTime', filters.metroTime);
            if (filters.developers.length > 0) {
                params.append('developers', filters.developers.join(','));
            }     
            if (filters.minFloor) params.append('minFloor', filters.minFloor);
            if (filters.maxFloor) params.append('maxFloor', filters.maxFloor);
            if (filters.minFloorCount) params.append('minFloorCount', filters.minFloorCount);
            if (filters.maxFloorCount) params.append('maxFloorCount', filters.maxFloorCount);
            if (filters.buildingStatus.length > 0) params.append('buildingStatus', filters.buildingStatus.join(','));

            params.append('page', 1);
            
            const response = await fetch(`${config.api.baseUrl}/flats?${params.toString()}`);
            
            if (response.ok) {
                const data = await response.json();
                allFlatsData = data.items;
                renderFlats(data.items);
                updatePagination(data.totalPages, 1);
                showNotification(`Фильтры применены. Найдено ${data.totalCount} квартир`, false);
                
                window.history.pushState({}, '', `?${params.toString()}`);
            } else {
                throw new Error('Ошибка загрузки данных');
            }
        } catch (error) {
            console.error('Ошибка при применении фильтров:', error);
            showNotification("Ошибка при применении фильтров", true);
        } finally {
            loadingIndicator.style.display = 'none';
        }
    });
    

    resetFiltersBtn.addEventListener('click', function() {
        document.getElementById('minFloor').value = '';
        document.getElementById('maxFloor').value = '';

        document.getElementById('minFloorCount').value = '';
        document.getElementById('maxFloorCount').value = '';
        
        document.querySelectorAll('.checkbox-input[name="roominess"]').forEach(el => {
            el.checked = false;
        });
        
        
        $('#metroSelect').val(null).trigger('change');
        $('#developerSelect').val(null).trigger('change');
        
        localStorage.removeItem('flatFilters');
        
        loadFlats(1);
        
        showNotification("Фильтры сброшены");
        
        $('#metroSelect').select2('close');
        $('#developerSelect').select2('close');
        
        document.querySelector('input[name="metroTime"][value="0"]').checked = true;
        
        document.querySelectorAll('.checkbox-input[name="buildingStatus"]').forEach(el => {
            el.checked = false;
        });
        
        localStorage.removeItem('flatFilters');
        
        loadFlats(1);
        showNotification("Фильтры сброшены", false);
    });
    

    function loadSavedFilters() {
        const savedFilters = JSON.parse(localStorage.getItem('flatFilters'));
        if (savedFilters) {
            if (savedFilters.minFloor) {
                document.getElementById('minFloor').value = savedFilters.minFloor;
            }
            
            if (savedFilters.maxFloor) {
                document.getElementById('maxFloor').value = savedFilters.maxFloor;
            }

            if (savedFilters.minFloorCount) {
                document.getElementById('minFloorCount').value = savedFilters.minFloorCount;
            }
            
            if (savedFilters.maxFloorCount) {
                document.getElementById('maxFloorCount').value = savedFilters.maxFloorCount;
            }
            
            if (savedFilters.roominess && savedFilters.roominess.length > 0) {
                savedFilters.roominess.forEach(value => {
                    const checkbox = document.querySelector(`.checkbox-input[name="roominess"][value="${value}"]`);
                    if (checkbox) checkbox.checked = true;
                });
            }
            
            if (savedFilters.metroStations && savedFilters.metroStations.length > 0) {
                $('#metroSelect').val(savedFilters.metroStations).trigger('change');
            }
            
            if (savedFilters.metroTime) {
                const radio = document.querySelector(`input[name="metroTime"][value="${savedFilters.metroTime}"]`);
                if (radio) radio.checked = true;
            }

            if (savedFilters.developers && savedFilters.developers.length > 0) {
                $('#developerSelect').val(savedFilters.developers).trigger('change');
            }
            
            if (savedFilters.buildingStatus && savedFilters.buildingStatus.length > 0) {
                savedFilters.buildingStatus.forEach(value => {
                    const checkbox = document.querySelector(`.checkbox-input[name="buildingStatus"][value="${value}"]`);
                    if (checkbox) checkbox.checked = true;
                });
            }
        }
    }


    loadFlats(currentPage);
    loadMetroStations();
    loadDevelopers();
    loadSavedFilters();
    renderComparisonList(); 

    
    window.addEventListener('popstate', function() {
        const urlParams = new URLSearchParams(window.location.search);
        const page = parseInt(urlParams.get('page')) || 1;
        if (page !== currentPage) {
            loadFlats(page);
        }
        setTimeout(100);
        renderComparisonList();
    });
});