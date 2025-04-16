export function initPasswordStrengthMeter() {
    const passwordInput = document.getElementById('password');
    if (!passwordInput) return;

    let container = passwordInput.closest('.password-container')
                         .querySelector('.password-strength-container');
    
    if (!container) {
        container = document.createElement('div');
        container.className = 'password-strength-container';
        passwordInput.closest('.password-container').appendChild(container);
    }

    const strengthMeter = document.createElement('div');
    strengthMeter.className = 'password-strength-meter';
    container.appendChild(strengthMeter);

    passwordInput.addEventListener('input', () => {
        const strength = calculatePasswordStrength(passwordInput.value);
        updateStrengthMeter(strengthMeter, strength);
    });
}

function calculatePasswordStrength(password) {
    let strength = 0;
    
    if (password.length >= 8) strength += 1;
    if (/[a-z]/.test(password) && /[A-Z]/.test(password)) strength += 1;
    if (/[0-9]/.test(password)) strength += 1;
    if (/[^A-Za-z0-9]/.test(password)) strength += 1;
    
    return strength;
}

function updateStrengthMeter(meter, strength) {
    meter.innerHTML = '';
    const strengthText = ['Очень слабый', 'Слабый', 'Средний', 'Хороший', 'Отличный'];
    
    for (let i = 0; i < 4; i++) {
        const bar = document.createElement('div');
        bar.className = `strength-bar ${i < strength ? 'active' : ''}`;
        bar.title = strengthText[strength];
        meter.appendChild(bar);
    }
    
    const text = document.createElement('div');
    text.className = 'strength-text';
    text.textContent = strengthText[strength];
    text.style.color = getStrengthColor(strength);
    meter.appendChild(text);
}

function getStrengthColor(strength) {
    const colors = ['#ff0000', '#ff5e00', '#ffbb00', '#a0ff00', '#00ff00'];
    return colors[strength] || '#cccccc';
}