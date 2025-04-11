import { checkTokenExpiration } from './token_сhecker.js';

export function updateAuthUI() {
  const isAuthenticated = checkTokenExpiration();
  const headerLink = document.querySelector('.header-link');
  const profileLink = document.querySelector('.profile-link-wrapper');

  if (!headerLink || !profileLink) {
    console.error('Не найдены элементы авторизации');
    return;
  }

  if (isAuthenticated) {
    headerLink.style.display = 'none';
    profileLink.style.display = 'block';
  } else {
    headerLink.style.display = 'flex';
    profileLink.style.display = 'none';
  }
}