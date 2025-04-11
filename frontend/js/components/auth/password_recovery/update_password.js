import { validatePassword, validateConfirmPassword } from '../../../utils/validation.js';
import { initPasswordStrengthMeter } from '../password_strength.js';
import { setupTogglePassword } from '../toggle_password.js';

export function setupPasswordValidation() {
  const passwordInput = document.getElementById('password');
  const confirmInput = document.getElementById('confirm-password');
  
  initPasswordStrengthMeter(passwordInput);
  setupTogglePassword();
  
  passwordInput.addEventListener('input', () => {
    const validation = validatePassword(passwordInput.value);
    updateValidationUI('password', validation);
  });
  
  confirmInput.addEventListener('input', () => {
    const validation = validateConfirmPassword(
      passwordInput.value, 
      confirmInput.value
    );
    updateValidationUI('confirm-password', validation);
  });
}

function updateValidationUI(fieldId, validation) {
  const errorElement = document.getElementById(`${fieldId}-error`);
  const input = document.getElementById(fieldId);
  
  if (!validation.isValid) {
    errorElement.textContent = validation.error;
    input.classList.add('error');
  } else {
    errorElement.textContent = '';
    input.classList.remove('error');
  }
}