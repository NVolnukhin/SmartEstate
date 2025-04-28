import { date } from '../../config.js';

document.addEventListener('DOMContentLoaded', () => {
  const dateElement = document.getElementById('actual-date');
  if (dateElement) {
    dateElement.textContent = date;
  }
});