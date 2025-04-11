import { generateClientHash } from '../../../utils/hashing.js';
import { config } from '../../../config.js';

export async function loginUser({ login, password }) {
  const clientHashedPassword = await generateClientHash(password);
  
  const response = await fetch(`${config.api.baseUrl}/api/users/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ login, password: clientHashedPassword })
  });

  const data = await response.json();
  
  if (!response.ok) {
    throw new Error(data.message || 'Ошибка авторизации');
  }

  return data;
}