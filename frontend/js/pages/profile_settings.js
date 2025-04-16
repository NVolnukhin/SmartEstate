import { initTokenChecker, checkTokenExpiration } from '../components/auth/token_сhecker.js';
import { showNotification, initNotification } from '../components/ui/notification.js';
import { config } from '../config.js';

document.addEventListener('DOMContentLoaded', function() {
    initNotification();
    loadUserData();
    initTokenChecker();

    document.querySelectorAll('.toggle-password').forEach(button => {
        button.addEventListener('click', function() {
            const targetId = this.getAttribute('data-target');
            const input = document.getElementById(targetId);
            if (input.type === 'password') {
                input.type = 'text';
                this.classList.remove('fa-eye-slash');
                this.classList.add('fa-eye');
            } else {
                input.type = 'password';
                this.classList.remove('fa-eye');
                this.classList.add('fa-eye-slash');
            }
        });
    });

    const newPasswordInput = document.getElementById('new-password');
    if (newPasswordInput) {
        newPasswordInput.addEventListener('input', function() {
            validatePasswordInRealTime(this.value);
        });
    }
});

function validatePasswordInRealTime(password) {
    const requirements = {
        length: password.length >= 8,
        uppercase: /[A-Z]/.test(password),
        lowercase: /[a-z]/.test(password),
        digit: /[0-9]/.test(password),
        special: /[^A-Za-z0-9]/.test(password)
    };

    Object.keys(requirements).forEach(key => {
        const element = document.getElementById(`req-${key}`);
        if (element) {
            element.classList.toggle('valid', requirements[key]);
        }
    });
}

async function loadUserData() {
    const token = localStorage.getItem('authToken');
    function checkAuth() {
        return localStorage.getItem('authToken') !== null && checkTokenExpiration();
    }

    if (!checkAuth()) {
        window.location.href = './login';
        return;
    } 
    
    try {
        const response = await fetch(`${config.api.baseUrl}/users/me`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        
        if (!response.ok) {
            throw new Error('Ошибка загрузки данных пользователя');
        }
        
        const userData = await response.json();
        
        document.getElementById('username-value').textContent = userData.login || 'Не указан';
        document.getElementById('name-value').textContent = userData.name || 'Не указано';
        document.getElementById('email-value').textContent = userData.email || 'Не указан';
        
        document.getElementById('name-input').value = userData.name || '';
        document.getElementById('email-input').value = userData.email || '';
        
    } catch (error) {
        console.error('Ошибка:', error);
        showNotification('Не удалось загрузить данные пользователя');
    }
}

window.showEditForm = function(field) {
    hideAllForms();
    document.getElementById(`${field}-form`).classList.add('active');
};

window.hideAllForms = function() {
    document.querySelectorAll('.edit-form').forEach(form => {
        form.classList.remove('active');
    });
};

function validatePassword(password) {
    const errors = [];
    if (password.length < 8) errors.push('Минимум 8 символов');
    if (!/[A-Z]/.test(password)) errors.push('Хотя бы одна заглавная буква');
    if (!/[a-z]/.test(password)) errors.push('Хотя бы одна строчная буква');
    if (!/[0-9]/.test(password)) errors.push('Хотя бы одна цифра');
    if (!/[^A-Za-z0-9]/.test(password)) errors.push('Хотя бы один специальный символ');
    
    return {
        isValid: errors.length === 0,
        errors: errors
    };
}

async function generateClientHash(password) {
    const clientSalt = 'fixed_client_salt_!@#';
    
    return sha256(password + clientSalt);
} 
window.saveChanges = async function(field) {
    const token = localStorage.getItem('authToken');
    if (!token) {
        window.location.href = './login';
        return;
    }
    
    let isValid = true;
    let endpoint = '';
    let body = {};
    
    try {
        switch (field) {
            case 'name':
                const newName = document.getElementById('name-input').value.trim();
                if (!newName) {
                    showNotification('Пожалуйста, введите имя');
                    isValid = false;
                    break;
                }

                if (newName.length < 3) {
                    showNotification('Имя должжно содержать не менее 3 символов');
                    isValid = false;
                    break;
                }
                
                endpoint = `${config.api.baseUrl}/users/name`;
                body = { newName: newName };
                break;
                
            case 'email':
                const newEmail = document.getElementById('email-input').value.trim();
                if (!newEmail) {
                    showNotification('Пожалуйста, введите email');
                    isValid = false;
                    break;
                }
                
                if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(newEmail)) {
                    showNotification('Пожалуйста, введите корректный email');
                    isValid = false;
                    break;
                }
                
                endpoint = `${config.api.baseUrl}/users/email`;
                body = { newEmail: newEmail };
                break;
                
            case 'password':
                const currentPassword = document.getElementById('current-password').value;
                const newPassword = document.getElementById('new-password').value;
                const confirmPassword = document.getElementById('confirm-password').value;

                const clientHashedCurrentPassword = await generateClientHash(currentPassword);
                const clientHashedNewPassword = await generateClientHash(newPassword);
                
                if (!currentPassword) {
                    isValid = false;
                }
                
                const passwordValidation = validatePassword(newPassword);
                if (!passwordValidation.isValid) {
                    const errorMessage = 'Требования к паролю не выполнены:\n' + 
                        passwordValidation.errors.map(err => `• ${err}`).join('\n');
                    showNotification(errorMessage);
                    isValid = false;
                }
                
                if (newPassword !== confirmPassword) {
                    showNotification('Введенные пароли не совпадают');
                    isValid = false;
                }
                
                if (!isValid) break;
                
                endpoint = `${config.api.baseUrl}/users/password`;
                body = {
                    currentPassword: clientHashedCurrentPassword,
                    newPassword: clientHashedNewPassword
                };
                break;
        }
        
        if (!isValid) return;
        
        const response = await fetch(endpoint, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(body)
        });
        
        if (!response.ok) {
            const errorText = await response.text();
            try {
                const errorData = JSON.parse(errorText);
                throw new Error(errorData.message || 'Ошибка сервера');
            } catch (e) {
                throw new Error(errorText || 'Ошибка сервера');
            }
        }
        
        await loadUserData();
        hideAllForms();
        
        if (field === 'password') {
            document.getElementById('current-password').value = '';
            document.getElementById('new-password').value = '';
            document.getElementById('confirm-password').value = '';
        }
        
        showNotification('Изменения успешно сохранены!', false);
    } catch (error) {
        console.error('Ошибка:', error);
        let errorMessage = "Неизвестная ошибка";
        
        try {
            const errorData = JSON.parse(error.message);
            
            if (Array.isArray(errorData) && errorData[0]?.message) {
                errorMessage = errorData[0].message;
            } 
            else if (errorData.message) {
                errorMessage = errorData.message;
            }
        } catch (e) {
            errorMessage = error.message || "Неизвестная ошибка";
        }
        
        showNotification(errorMessage);
    }
}    