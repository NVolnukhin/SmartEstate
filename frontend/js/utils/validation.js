export function validateEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!re.test(email)) {
      return { isValid: false, error: 'Введите корректный email' };
    }
    return { isValid: true };
  }
  
  export function validatePassword(password) {
    const errors = [];
    if (password.length < 8) errors.push('Минимум 8 символов');
    if (!/[A-Z]/.test(password)) errors.push('Хотя бы одна заглавная буква');
    if (!/[0-9]/.test(password)) errors.push('Хотя бы одна цифра');
    
    return {
      isValid: errors.length === 0,
      errors: errors.length ? errors : null,
      error: errors.join(', ')
    };
  }
  
  export function validateConfirmPassword(password, confirmPassword) {
    if (password !== confirmPassword) {
      return { isValid: false, error: 'Пароли не совпадают' };
    }
    return { isValid: true };
  }