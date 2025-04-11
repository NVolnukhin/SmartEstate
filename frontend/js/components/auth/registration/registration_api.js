import { generateClientHash } from '../../../utils/hashing.js';
import { config } from '../../../config.js';

export async function registerUser({ username, email, password }) {
  const clientHashedPassword = await generateClientHash(password);
  
  const response = await fetch(`${config.api.baseUrl}/users/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      login: username,
      email: email,
      name: username,
      password: clientHashedPassword
    })
  });

  const data = await response.json();
  
  if (!response.ok) {
    throw new Error(data.message || 'Пользователь с данным логином или e-mail уже существует');
  }

  localStorage.setItem('authToken', data.token);
  return data;
}