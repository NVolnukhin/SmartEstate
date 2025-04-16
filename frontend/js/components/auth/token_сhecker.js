function checkTokenExpiration() {
    const token = localStorage.getItem('authToken');
    if (!token) return false;
    
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const expiresAt = payload.exp;
        const now = Math.floor(Date.now() / 1000);

        if (now > expiresAt) {
            localStorage.removeItem('authToken');
            showSessionExpiredModal();
            
            return false;
        }
        
        return true;
    } catch (error) {
        console.error('Ошибка при проверке токена:', error);
        localStorage.removeItem('authToken');
        return false;
    }
}

function showSessionExpiredModal() {
    if (document.getElementById('sessionExpiredModal')) return;
    
    const modal = document.createElement('div');
    modal.id = 'sessionExpiredModal';
    modal.style.position = 'fixed';
    modal.style.top = '0';
    modal.style.left = '0';
    modal.style.width = '100%';
    modal.style.height = '100%';
    modal.style.backgroundColor = 'rgba(0,0,0,0.7)';
    modal.style.zIndex = '1000';
    modal.style.display = 'flex';
    modal.style.justifyContent = 'center';
    modal.style.alignItems = 'center';
    modal.style.backdropFilter = 'blur(3px)';
    
    modal.innerHTML = `
        <div style="
            background: #FFE4AA; 
            padding: 25px; 
            border-radius: 12px; 
            max-width: 400px; 
            width: 90%;
            text-align: center;
            position: relative;
            box-shadow: 0 4px 20px rgba(0,0,0,0.2);
        ">
            <button class="close-modal-btn" style="
                position: absolute;
                top: 10px;
                right: 10px;
                background: none;
                border: none;
                font-size: 20px;
                cursor: pointer;
                color: #40027E;
                padding: 5px;
            ">
                &times;
            </button>
            
            <h3 style="color: #40027E; margin-top: 10px; margin-bottom: 20px;">
                Сессия истекла
            </h3>
            
            <p style="margin: 15px 0; color: #5E04B5; line-height: 1.5;">
                Ваша сессия истекла. Пожалуйста, войдите снова.
            </p>
            
            <div style="display: flex; gap: 15px; justify-content: center; margin-top: 25px;">
                <button class="modal-login-btn" style="
                    background: #40027E; 
                    color: #FFE4AA; 
                    border: none; 
                    padding: 12px 25px; 
                    border-radius: 6px; 
                    cursor: pointer;
                    font-weight: 600;
                    transition: all 0.3s ease;
                    flex: 1;
                ">
                    Войти
                </button>
            </div>
        </div>
    `;
    
    document.body.appendChild(modal);
    document.body.style.overflow = 'hidden'; 
    
    modal.querySelector('.close-modal-btn').addEventListener('click', () => {
        closeModal();
    });
    
    modal.querySelector('.modal-login-btn').addEventListener('click', () => {
        window.location.href = './login?session_expired=true';
    });
    
    function closeModal() {
        document.body.style.overflow = ''; 
        modal.style.opacity = '0';
        modal.style.transition = 'opacity 0.3s ease';
        
        setTimeout(() => {
            if (modal.parentNode) {
                modal.parentNode.removeChild(modal);
            }
        }, 300);
    }
    
    modal.addEventListener('click', (e) => {
        if (e.target === modal) {
            closeModal();
        }
    });
    
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            closeModal();
        }
    });
}

async function initTokenChecker(interval = 300000) {
    checkTokenExpiration();
    setInterval(checkTokenExpiration, interval);
}

export { checkTokenExpiration, initTokenChecker };