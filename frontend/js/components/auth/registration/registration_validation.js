import { validateEmail, validatePassword, validateConfirmPassword } from "../../../utils/validation.js";

export function validateRegistrationForm({ username, email, password, confirmPassword }) {
    if (!username) {
      return { isValid: false, message: 'Введите логин' };
    }
  
    const emailValidation = validateEmail(email);
    if (!emailValidation.isValid) {
      return emailValidation;
    }
  
    const passwordValidation = validatePassword(password);
    if (!passwordValidation.isValid) {
      return passwordValidation;
    }
  
    const confirmValidation = validateConfirmPassword(password, confirmPassword);
    if (!confirmValidation.isValid) {
      return confirmValidation;
    }
  
    return { isValid: true };
  }