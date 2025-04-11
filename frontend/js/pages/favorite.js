import { initTokenChecker, checkTokenExpiration } from '../components/auth/token_сhecker.js';
import { config } from '../config.js';

        document.addEventListener('DOMContentLoaded', function() {
            initTokenChecker();

            function checkAuth() {
                return localStorage.getItem('authToken') !== null && checkTokenExpiration();
            }
            
            const urlParams = new URLSearchParams(window.location.search);
            let currentPage = parseInt(urlParams.get('page')) || 1;
            const pageSize = 10;
            let totalPages = 1;
            const loadingIndicator = document.getElementById('loading-indicator');
            
            async function loadFavorites(page = 1) {
                if (!checkAuth()) {
                    window.location.href = 'login.html';
                    return;
                }
                
                loadingIndicator.style.display = 'block';
                currentPage = page;
                
                window.history.pushState({}, '', `?page=${page}`);
                
                try {
                    const response = await fetch(`${config.api.baseUrl}/api/user-preferences/paged-favorites?page=${page}&pageSize=${pageSize}`, {
                        headers: {
                            'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                        }
                    });
                    
                    if (response.ok) {
                        const data = await response.json();
                        renderFlats(data.items);
                        updatePagination(data.totalPages, page);
                    } else {
                        console.error('Ошибка загрузки избранного:', response.statusText);
                        renderEmptyState(true);
                    }
                } catch (error) {
                    console.error('Ошибка соединения:', error);
                    renderEmptyState(true);
                } finally {
                    loadingIndicator.style.display = 'none';
                }
            }
            
            function renderFlats(favorites) {
                const container = document.getElementById('flatsList');
                container.innerHTML = '';

                if (!favorites || favorites.length === 0) {
                    renderEmptyState();
                    return;
                }
                
                favorites.forEach(favorite => {
                    const flat = favorite.flatInfo;
                    const card = document.createElement('div');
                    card.className = 'flat-card';
                    card.setAttribute('data-id', flat.flatId);
                    
                    const formattedPrice = new Intl.NumberFormat('ru-RU', {
                        style: 'currency',
                        currency: 'RUB',
                        maximumFractionDigits: 0
                    }).format(flat.price).replace('RUB', '₽');
                    
                    const mainImage = flat.mainImage || 
                        'https://via.placeholder.com/240x220/40027E/FFE4AA?text=No+Image';
                    
                    card.innerHTML = `
                        <div class="flat-image">
                            <img src="${mainImage}" alt="ЖК ${flat.residentialComplex}">
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
                                    <div>${flat.roominess}-комнатная</div>
                                </div>
                                <div class="detail-item">
                                    <div class="detail-label">Этаж</div>
                                    <div>${flat.floor}</div>
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
                                    <div>${flat.residentialComplex}</div>
                                </div>
                            </div>
                        </div>
                        <div class="flat-actions">
                            <button class="action-btn favorite-btn active" data-id="${flat.flatId}">
                                <i class="fas fa-heart"></i> В избранном
                            </button>
                        </div>
                    `;
                    
                    container.appendChild(card);
                });
                
                initFavoriteButtons();
                
                document.querySelectorAll('.flat-card').forEach(card => {
                    card.addEventListener('click', function(e) {
                        if (e.target.closest('.action-btn')) {
                            return;
                        }
                        
                        const flatId = this.getAttribute('data-id');
                        goToFlatPage(flatId);
                    });
                });
            }

            function updatePagination(totalPagesCount, currentPage) {
                totalPages = totalPagesCount;
                const pagination = document.getElementById('pagination');
                pagination.innerHTML = '';
                
                if (totalPages <= 1) return;
                
                const prevBtn = document.createElement('button');
                prevBtn.className = 'pagination-btn';
                prevBtn.innerHTML = '<i class="fas fa-chevron-left"></i>';
                prevBtn.disabled = currentPage === 1;
                prevBtn.addEventListener('click', () => {
                    if (currentPage > 1) {
                        loadFavorites(currentPage - 1);
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
                            loadFavorites(i);
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
                        loadFavorites(currentPage + 1);
                    }
                });
                pagination.appendChild(nextBtn);
            }
            
            function renderEmptyState(isError = false) {
                const container = document.getElementById('flatsList');
                container.innerHTML = `
                    <div class="empty-favorites">
                        ${isError ? 
                            '<p>Произошла ошибка при загрузке избранного</p>' : 
                            '<p>В избранном пока нет квартир</p>'}
                        <a href="./all_flats.html">Перейти к поиску квартир</a>
                    </div>
                `;
                
                document.getElementById('pagination').innerHTML = '';
            }
            

            function initFavoriteButtons() {
                document.querySelectorAll('.favorite-btn').forEach(btn => {
                    btn.addEventListener('click', async function(e) {
                        e.stopPropagation();
                        
                        const flatId = this.getAttribute('data-id');
                        const success = await removeFromFavorites(flatId);
                        
                        if (success) {
                            this.closest('.flat-card').remove();
                            
                            if (document.querySelectorAll('.flat-card').length === 0) {
                                renderEmptyState();
                            }
                        }
                    });
                });
            }
            

            async function removeFromFavorites(flatId) {
                try {
                    const response = await fetch(`${config.api.baseUrl}/api/user-preferences/favorites/${flatId}`, {
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
                window.location.href = `flat.html?id=${flatId}`;
            }
            
            if (!checkAuth()) {
                window.location.href = 'login.html';
            } else {
                loadFavorites(currentPage);
            }
            

            window.addEventListener('popstate', function() {
                const urlParams = new URLSearchParams(window.location.search);
                const page = parseInt(urlParams.get('page')) || 1;
                if (page !== currentPage) {
                    loadFavorites(page);
                }
            });
        });