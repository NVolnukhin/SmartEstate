import { showNotification } from '../../ui/notification.js';
import { validateRegistrationForm } from './registration_validation.js';
import { registerUser } from './registration_api.js';
import { setupTogglePassword } from '../toggle_password.js';
import { initPasswordStrengthMeter } from '../password_strength.js';
import { validateEmail, validatePassword, validateConfirmPassword } from "../../../utils/validation.js";

export class RegistrationManager {
  constructor() {
    this.form = document.getElementById('registrationForm');
    this.submitButton = document.querySelector('.form-button');
    this.init();
  }

  init() {
    this.setupEventListeners();
    setupTogglePassword();
    initPasswordStrengthMeter();
  }

  setupEventListeners() {
    this.form.addEventListener('submit', async (e) => {
      e.preventDefault();
      await this.handleSubmit();
    });
  
    document.getElementById('username').addEventListener('input', (e) => {
      this.validateUsername(e.target.value);
    });
  
    document.getElementById('email').addEventListener('input', (e) => {
      this.validateEmail(e.target.value);
    });
  
    document.getElementById('password').addEventListener('input', (e) => {
      this.validatePassword(e.target.value);
      const confirmPassword = document.getElementById('confirm-password').value;
      if (confirmPassword) {
        this.validateConfirmPassword(e.target.value, confirmPassword);
      }
    });
  
    document.getElementById('confirm-password').addEventListener('input', (e) => {
      const password = document.getElementById('password').value;
      this.validateConfirmPassword(password, e.target.value);
    });
  }
  

  async handleSubmit() {
    const formData = this.getFormData();
    const validation = validateRegistrationForm(formData);

    if (!validation.isValid) {
      showNotification(validation.message);
      return;
    }

    try {
      this.setLoadingState(true);
      await registerUser(formData);
      showNotification('Регистрация прошла успешно!', false);
      setTimeout(() => window.location.href = './index', 1500);
    } catch (error) {
      showNotification(error.message);
    } finally {
      this.setLoadingState(false);
    }
  }

  getFormData() {
    return {
      username: document.getElementById('username').value.trim(),
      email: document.getElementById('email').value.trim(),
      password: document.getElementById('password').value.trim(),
      confirmPassword: document.getElementById('confirm-password').value.trim()
    };
  }

  setLoadingState(isLoading) {
    this.submitButton.textContent = isLoading ? 'Регистрация...' : 'Зарегистрироваться';
    this.submitButton.disabled = isLoading;
  }

  validateUsername(value) {
    const isValid = value.trim().length > 0;
    this.updateFieldValidation('username', isValid, 'Введите логин');
  }
  
  validateEmail(value) {
    const validation = validateEmail(value);
    this.updateFieldValidation('email', validation.isValid, validation.error);
  }
  
  validatePassword(value) {
    const validation = validatePassword(value);
    if (validation.errors) {
      this.showPasswordErrors(validation.errors);
    } else {
      this.updateFieldValidation('password', true);
    }
  }
  
  validateConfirmPassword(password, confirmPassword) {
    const validation = validateConfirmPassword(password, confirmPassword);
    this.updateFieldValidation('confirm-password', validation.isValid, validation.error);
  }
  
  updateFieldValidation(fieldId, isValid, errorMessage = '') {
    const errorElement = document.getElementById(`${fieldId}-error`);
    const hintElement = document.getElementById(`${fieldId}-hint`);
    const input = document.getElementById(fieldId);
  
    if (!isValid) {
      errorElement.textContent = errorMessage;
      hintElement.style.display = 'block';
      input.classList.add('error');
    } else {
      hintElement.style.display = 'none';
      input.classList.remove('error');
    }
  }
  
  showPasswordErrors(errors) {
    const errorElement = document.getElementById('password-error');
    const hintElement = document.getElementById('password-hint');
    const input = document.getElementById('password');
  
    if (errors && errors.length > 0) {
      errorElement.innerHTML = `
        <strong>Требования к паролю:</strong>
        <ul>${errors.map(e => `<li>${e}</li>`).join('')}</ul>
      `;
      hintElement.style.display = 'block';
      input.classList.add('error');
    } else {
      hintElement.style.display = 'none';
      input.classList.remove('error');
    }
  }
}