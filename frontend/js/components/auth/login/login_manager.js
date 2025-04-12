import { showNotification } from '../../ui/notification.js';
import { loginUser } from './login_api.js';
import { setupTogglePassword } from '../toggle_password.js';
import { generateClientHash } from '../../../utils/hashing.js';

export class LoginManager {
  constructor() {
    this.form = document.getElementById('loginForm');
    this.loginInput = document.getElementById('login');
    this.passwordInput = document.getElementById('password');
    this.submitButton = document.querySelector('.form-button');
    this.init();
  }

  init() {
    this.setupEventListeners();
    this.setupTogglePassword();
  }

  setupEventListeners() {
    this.form.addEventListener('submit', async (e) => {
      e.preventDefault();
      await this.handleLogin();
    });

    // Real-time валидация
    [this.loginInput, this.passwordInput].forEach(input => {
      input.addEventListener('input', () => this.validateField(input));
    });

    // Обработчик "Забыли пароль"
    document.querySelector('.forgot-password')?.addEventListener('click', (e) => {
      e.preventDefault();
      window.location.href = './forgot_password';
    });
  }

  setupTogglePassword() {
    document.querySelectorAll('.toggle-password').forEach(button => {
      button.addEventListener('click', function() {
        const input = this.parentElement.querySelector('.form-input');
        const isPassword = input.type === 'password';
        input.type = isPassword ? 'text' : 'password';
        this.classList.toggle('fa-eye-slash', isPassword);
        this.classList.toggle('fa-eye', !isPassword);
      });
    });
  }

  async handleLogin() {
    if (!this.validateForm()) return;

    try {
      this.setLoadingState(true);
      const response = await loginUser({
        login: this.loginInput.value.trim(),
        password: this.passwordInput.value.trim()
      });
      
      localStorage.setItem('authToken', response.token);
      window.location.href = './index';
    } catch (error) {
      showNotification(error.message);
    } finally {
      this.setLoadingState(false);
    }
  }

  validateForm() {
    let isValid = true;
    
    // Сброс ошибок
    document.querySelectorAll('.error-message').forEach(el => el.style.display = 'none');
    document.querySelectorAll('.form-input').forEach(el => el.classList.remove('error'));

    // Валидация логина
    if (!this.loginInput.value.trim()) {
      this.showError(this.loginInput, 'login-error', 'Введите логин');
      isValid = false;
    }

    // Валидация пароля
    if (!this.passwordInput.value.trim()) {
      this.showError(this.passwordInput, 'password-error', 'Введите пароль');
      isValid = false;
    }

    return isValid;
  }

  validateField(input) {
    if (input.value.trim()) {
      input.classList.remove('error');
      document.getElementById(`${input.id}-error`).style.display = 'none';
    }
  }

  showError(input, errorId, message) {
    input.classList.add('error');
    const errorElement = document.getElementById(errorId);
    errorElement.textContent = message;
    errorElement.style.display = 'block';
  }

  setLoadingState(isLoading) {
    this.submitButton.textContent = isLoading ? 'Вход...' : 'Войти';
    this.submitButton.disabled = isLoading;
  }
}