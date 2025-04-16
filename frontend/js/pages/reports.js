import { ReportsManager } from '../components/reports/report_manager.js';
import { initTokenChecker } from '../components/auth/token_сhecker.js';
import { updateAuthUI } from '../components/auth/update_auth_ui.js';

document.addEventListener('DOMContentLoaded', function() {
  try {
    new ReportsManager();
    
    initTokenChecker()
      .then(() => {
        updateAuthUI();
      })
      .catch(error => {
        console.error('Ошибка инициализации проверки токена:', error);
        updateAuthUI();
      });

    window.addEventListener('storage', updateAuthUI);
  } catch (error) {
    console.error('Ошибка инициализации страницы отчетов:', error);
  }
});