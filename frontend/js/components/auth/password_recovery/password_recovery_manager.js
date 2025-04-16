import { showNotification } from '../../ui/notification.js';
import { requestPasswordRecovery } from './password_recovery_api.js';
import { validateEmail } from '../../../utils/validation.js';
import { config } from '/js/config.js';

export class PasswordRecoveryManager {
  constructor() {
    this.form = document.getElementById('forgotPasswordForm');
    this.emailInput = document.getElementById('email');
    this.submitButton = document.querySelector('.form-button');
    this.init();
  }

  init() {
    this.setupEventListeners();
  }

  setupEventListeners() {
    this.form.addEventListener('submit', async (e) => {
      e.preventDefault();
      await this.handlePasswordRecovery();
    });

    this.emailInput.addEventListener('input', () => this.validateEmail());
  }

  async handlePasswordRecovery() {
    if (!this.validateForm()) return;

    try {
      this.setLoadingState(true);
      await requestPasswordRecovery(this.emailInput.value.trim());
      
      showNotification('Письмо с инструкциями отправлено на вашу почту', false);
      setTimeout(() => {
        window.location.href = './index';
      }, 3000);
    } catch (error) {
      showNotification(error.message);
    } finally {
      this.setLoadingState(false);
    }
  }

  validateForm() {
    const validation = validateEmail(this.emailInput.value);
    
    if (!validation.isValid) {
      showNotification("Введите существующий e-mail")
      return false;
    }
    
    return true;
  }

  validateEmail() {
    if (this.emailInput.value.length === 0) {
      this.hideError();
      return;
    }
  }

  setLoadingState(isLoading) {
    this.submitButton.textContent = isLoading ? 'Отправка...' : 'Отправить инструкции';
    this.submitButton.disabled = isLoading;
  }
}