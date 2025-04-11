import { setupTogglePassword } from '../components/auth/toggle_password.js';
import { initPasswordStrengthMeter } from '../components/auth/password_strength.js';
import { validatePassword, validateConfirmPassword } from '../utils/validation.js';
import { showNotification, initNotification } from '../components/ui/notification.js';
import { config } from '../config.js';

document.addEventListener('DOMContentLoaded', function() {
    initNotification();
    setupTogglePassword();
    initPasswordStrengthMeter();

    const urlParams = new URLSearchParams(window.location.search);
    const token = urlParams.get('token');
    if (token) {
        document.getElementById('token').value = token;
    } else {
        showNotification('Недействительная ссылка для восстановления пароля');
        return;
    }

    const form = document.getElementById('recoveryForm');
    const passwordInput = document.getElementById('password');
    const confirmPasswordInput = document.getElementById('confirm-password');
    const submitButton = document.querySelector('.form-button');

    form.addEventListener('submit', async function(e) {
        e.preventDefault();
        resetErrors();
        
        let isValid = true;
        
        const passwordValidation = validatePassword(passwordInput.value);
        if (!passwordValidation.isValid) {
            showPasswordErrors(passwordValidation.errors);
            isValid = false;
        }
        
        const confirmValidation = validateConfirmPassword(
            passwordInput.value, 
            confirmPasswordInput.value
        );
        if (!confirmValidation.isValid) {
            showNotification(confirmValidation.error);
            isValid = false;
        }
        
        if (!isValid) return;
        
        submitButton.textContent = 'Обновление...';
        submitButton.disabled = true;
        
        async function generateClientHash(password) {
            const clientSalt = 'fixed_client_salt_!@#';
            
            return sha256(password + clientSalt);
        } 

        try {
            const clientHashedPassword = await generateClientHash(passwordInput.value);
            const clientHashedPasswordConfirmation = await generateClientHash(confirmPasswordInput.value);

            const response = await fetch(`${config.api.baseUrl}/password-recovery/confirm`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },

                body: JSON.stringify({
                    token: document.getElementById('token').value,
                    newPassword: clientHashedPassword,
                    confirmPassword: clientHashedPasswordConfirmation
                })
            });
            
            const data = await response.json();
            
            if (response.ok) {
                showNotification('Пароль успешно изменен', false);
                setTimeout(() => {
                    window.location.href = './login.html';
                }, 3000);
            } else {
                throw new Error(data.message || 'Ошибка при сбросе пароля');
            }
        } catch (error) {
            let errorMessage = 'Произошла неизвестная ошибка';
            
            try {
                const data = await response.json();
                
                if (data.errors && data.errors.length > 0) {
                    errorMessage = data.errors[0].message;
                } else if (data.message) {
                    errorMessage = data.message;
                }
            } catch (parseError) {
                errorMessage = error.message;
            }
            
            showNotification(errorMessage);
            submitButton.textContent = 'Сбросить пароль';
            submitButton.disabled = false;
        }
    });
    
    passwordInput.addEventListener('input', function() {
        const validation = validatePassword(this.value);
        updatePasswordValidationUI(validation);
        
        if (confirmPasswordInput.value) {
            const confirmValidation = validateConfirmPassword(
                this.value, 
                confirmPasswordInput.value
            );
            updateValidationUI('confirm-password', confirmValidation);
        }
    });
    
    confirmPasswordInput.addEventListener('input', function() {
        const validation = validateConfirmPassword(
            passwordInput.value, 
            this.value
        );
        updateValidationUI('confirm-password', validation);
    });
});

function showPasswordErrors(errors) {
    const errorElement = document.getElementById('password-error');
    const hintElement = document.getElementById('password-hint');
    
    errorElement.innerHTML = `
        <strong>Требования к паролю:</strong>
        <ul>${errors.map(e => `<li>${e}</li>`).join('')}</ul>
    `;
    hintElement.style.display = 'block';
    document.getElementById('password').classList.add('error');
}

function resetErrors() {
    document.querySelectorAll('.error-hint').forEach(el => {
        el.style.display = 'none';
        el.querySelector('.error').textContent = '';
    });
    document.querySelectorAll('.form-input').forEach(el => {
        el.classList.remove('error');
    });
}

function updateValidationUI(fieldId, validation) {
    const errorElement = document.getElementById(`${fieldId}-error`);
    const hintElement = document.getElementById(`${fieldId}-hint`);
    const input = document.getElementById(fieldId);
    
    if (input.value.length === 0) {
        hintElement.style.display = 'none';
        input.classList.remove('error');
        return;
    }
    
    if (!validation.isValid) {
        errorElement.textContent = validation.error;
        hintElement.style.display = 'block';
        input.classList.add('error');
    } else {
        hintElement.style.display = 'none';
        input.classList.remove('error');
    }
}

function updatePasswordValidationUI(validation) {
    const errorElement = document.getElementById('password-error');
    const hintElement = document.getElementById('password-hint');
    const input = document.getElementById('password');
    
    if (input.value.length === 0) {
        hintElement.style.display = 'none';
        input.classList.remove('error');
        return;
    }
    
    if (!validation.isValid) {
        errorElement.innerHTML = `
            <strong>Требования к паролю:</strong>
            <ul>${validation.errors.map(e => `<li>${e}</li>`).join('')}</ul>
        `;
        hintElement.style.display = 'block';
        input.classList.add('error');
    } else {
        hintElement.style.display = 'none';
        input.classList.remove('error');
    }
}