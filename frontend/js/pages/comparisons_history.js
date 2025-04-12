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
            let currentPage = parseInt(urlParams.get('page')) || 1;
            const pageSize = 15;
            let totalPages = 1;
            const loadingIndicator = document.getElementById('loading-indicator');
            
            async function loadComparisonHistory(page = 1) {
                if (!checkAuth()) {
                    window.location.href = 'login';
                    return;
                }
                
                loadingIndicator.style.display = 'block';
                currentPage = page;
                
                window.history.pushState({}, '', `?page=${page}`);
                
                try {
                    const response = await fetch(`${config.api.baseUrl}/user-preferences/paged-comparisons?page=${page}&pageSize=${pageSize}`, {
                        headers: {
                            'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                        }
                    });
                    
                    if (response.ok) {
                        const data = await response.json();
                        renderHistory(data.items);
                        updatePagination(data.totalPages, page);
                    } else {
                        console.error('Ошибка загрузки истории сравнений:', response.statusText);
                        renderEmptyState(true);
                    }
                } catch (error) {
                    console.error('Ошибка соединения:', error);
                    renderEmptyState(true);
                } finally {
                    loadingIndicator.style.display = 'none';
                }
            }
            
            function renderHistory(comparisons) {
                const historyList = document.getElementById('historyList');
                const emptyHistory = document.getElementById('emptyHistory');
                historyList.innerHTML = '';

                if (!comparisons || comparisons.length === 0) {
                    renderEmptyState();
                    return;
                }
                
                emptyHistory.style.display = 'none';
                
                comparisons.forEach(comparison => {
                    const flat1 = comparison.flat1;
                    const flat2 = comparison.flat2;
                    const formatPrice = (price) => {
                        return new Intl.NumberFormat('ru-RU', {
                            style: 'currency',
                            currency: 'RUB',
                            maximumFractionDigits: 0
                        }).format(price).replace('RUB', '₽');
                    };
                    
                    const historyItem = document.createElement('div');
                    historyItem.className = 'history-item';
                    historyItem.innerHTML = `
                        <button class="delete-comparison" data-id="${comparison.comparisonId}">
                            <i class="fas fa-trash-alt"></i> Удалить
                        </button>
                        <div class="flats-preview">
                            <div class="flat-preview" data-id="${flat1.flatId}">
                                <div class="flat-miniature">
                                    <img src="${flat1.mainImage || 'https://via.placeholder.com/300x200/40027E/FFE4AA?text=No+Image'}" alt="${flat1.residentialComplex}">
                                </div>
                                <div class="flat-info">
                                    <div class="flat-price">${formatPrice(flat1.price)}</div>
                                    <div class="flat-details">
                                        <div class="detail-item">
                                            <div class="detail-label">Площадь</div>
                                            <div class="detail-value">${flat1.square} м²</div>
                                        </div>
                                        <div class="detail-item">
                                            <div class="detail-label">Комнатность</div>
                                            <div class="detail-value">${flat1.roominess}-комнатная</div>
                                        </div>
                                        <div class="detail-item">
                                            <div class="detail-label">Этаж</div>
                                            <div class="detail-value">${flat1.floor}</div>
                                        </div>
                                        <div class="detail-item">
                                            <div class="detail-label">Метро</div>
                                            <div class="detail-value">
                                                <i class="fas fa-subway metro-icon"></i>
                                                ${flat1.nearestMetro.name} (${flat1.nearestMetro.minutesToMetro} мин)
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="flat-preview" data-id="${flat2.flatId}">
                                <div class="flat-miniature">
                                    <img src="${flat2.mainImage || 'https://via.placeholder.com/300x200/40027E/FFE4AA?text=No+Image'}" alt="${flat2.residentialComplex}">
                                </div>
                                <div class="flat-info">
                                    <div class="flat-price">${formatPrice(flat2.price)}</div>
                                    <div class="flat-details">
                                        <div class="detail-item">
                                            <div class="detail-label">Площадь</div>
                                            <div class="detail-value">${flat2.square} м²</div>
                                        </div>
                                        <div class="detail-item">
                                            <div class="detail-label">Комнатность</div>
                                            <div class="detail-value">${flat2.roominess}-комнатная</div>
                                        </div>
                                        <div class="detail-item">
                                            <div class="detail-label">Этаж</div>
                                            <div class="detail-value">${flat2.floor}</div>
                                        </div>
                                        <div class="detail-item">
                                            <div class="detail-label">Метро</div>
                                            <div class="detail-value">
                                                <i class="fas fa-subway metro-icon"></i>
                                                ${flat2.nearestMetro.name} (${flat2.nearestMetro.minutesToMetro} мин)
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="compare-actions">
                            <button class="compare-btn" data-id1="${flat1.flatId}" data-id2="${flat2.flatId}">
                                <i class="fas fa-balance-scale"></i> Перейти к сравнению
                            </button>
                        </div>
                    `;
                    
                    historyItem.querySelectorAll('.flat-preview').forEach(preview => {
                        preview.addEventListener('click', function(e) {
                            if (e.target.closest('.delete-comparison') || e.target.closest('.compare-btn')) {
                                return;
                            }
                            const flatId = this.getAttribute('data-id');
                            goToFlatPage(flatId);
                        });
                    });
                    
                    historyItem.querySelector('.compare-btn').addEventListener('click', function() {
                        const id1 = this.getAttribute('data-id1');
                        const id2 = this.getAttribute('data-id2');
                        goToComparisonPage(id1, id2);
                    });
                    
                    historyItem.querySelector('.delete-comparison').addEventListener('click', async function(e) {
                        e.stopPropagation();
                        const comparisonId = this.getAttribute('data-id');
                        await deleteComparison(comparisonId);
                    });
                    
                    historyList.appendChild(historyItem);
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
                        loadComparisonHistory(currentPage - 1);
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
                            loadComparisonHistory(i);
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
                        loadComparisonHistory(currentPage + 1);
                    }
                });
                pagination.appendChild(nextBtn);
            }
            
            function renderEmptyState(isError = false) {
                const historyList = document.getElementById('historyList');
                const emptyHistory = document.getElementById('emptyHistory');
                
                historyList.innerHTML = '';
                emptyHistory.style.display = 'block';
                emptyHistory.innerHTML = `
                    <p>${isError ? 'Произошла ошибка при загрузке истории' : 'Вы пока не сравнивали квартиры'}</p>
                    <a href="./all_flats">Перейти к списку квартир</a>
                `;
                
                document.getElementById('pagination').innerHTML = '';
            }
            
            async function deleteComparison(comparisonId) {
                try {
                    const response = await fetch(`${config.api.baseUrl}/user-preferences/comparisons/${comparisonId}`, {
                        method: 'DELETE',
                        headers: {
                            'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                        }
                    });
                    
                    if (response.ok) {
                        loadComparisonHistory(currentPage);
                    } else {
                        console.error('Ошибка удаления сравнения:', response.statusText);
                    }
                } catch (error) {
                    console.error('Ошибка соединения:', error);
                }
            }
            
            function goToFlatPage(flatId) {
                window.location.href = `flat?id=${flatId}`;
            }
            
            function goToComparisonPage(id1, id2) {
                window.location.href = `compare?id1=${id1}&id2=${id2}`;
            }
            
            if (!checkAuth()) {
                window.location.href = 'login';
            } else {
                loadComparisonHistory(currentPage);
            }
            
            window.addEventListener('popstate', function() {
                const urlParams = new URLSearchParams(window.location.search);
                const page = parseInt(urlParams.get('page')) || 1;
                if (page !== currentPage) {
                    loadComparisonHistory(page);
                }
            });
        });