export function showNotification(message, isError = true, duration = 3000) {
    const notification = document.getElementById('global-notification');
    const messageElement = document.getElementById('notification-message');
    
    if (!notification || !messageElement) {
        console.error('Элементы уведомления не найдены в DOM');
        return;
    }
    
    messageElement.textContent = message;
    notification.style.whiteSpace = 'pre-line';
    notification.className = isError ? 'notification error' : 'notification success';
    notification.classList.add('show');
    
    setTimeout(() => {
        notification.classList.remove('show');
    }, duration);
}

export function initNotification() {
    if (!document.getElementById('global-notification')) {
        const notificationHTML = `
            <div class="notification" id="global-notification">
                <i class="fas fa-exclamation-circle"></i>
                <span id="notification-message"></span>
            </div>
        `;
        document.body.insertAdjacentHTML('beforeend', notificationHTML);
    }
}