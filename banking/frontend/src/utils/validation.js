export function isValidEmail(email) {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

export function isFormComplete(formData) {
  return Object.values(formData).every((value) => String(value).trim().length > 0);
}
