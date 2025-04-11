export async function requestPasswordRecovery(email) {
    const response = await fetch(`${config.api.baseUrl}/password-recovery/request`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ email })
    });
  
    const data = await response.json();
    
    if (!response.ok) {
      throw new Error(data.message || 'Ошибка при отправке запроса');
    }
  
    return data;
  }