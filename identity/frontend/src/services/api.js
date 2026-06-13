const API_BASE = import.meta.env.VITE_IDENTITY_API_URL ?? '';

function getToken() {
  return localStorage.getItem('nagarik_token');
}

function authHeaders() {
  const token = getToken();
  return {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  };
}

async function handleResponse(response) {
  if (!response.ok) {
    const error = await response.json().catch(() => ({}));
    const message = error.errors?.[0] || error.message || `Request failed (${response.status})`;
    throw new Error(message);
  }
  return response.json();
}

export async function login(username, password) {
  const response = await fetch(`${API_BASE}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, password }),
  });

  const data = await handleResponse(response);
  localStorage.setItem('nagarik_token', data.token);
  localStorage.setItem('nagarik_user', JSON.stringify({
    token: data.token,
    userId: data.userId ?? data.UserId,
    fullName: data.fullName ?? data.FullName,
    expiresAt: data.expiresAt ?? data.ExpiresAt,
  }));
  return data;
}

export function logout() {
  localStorage.removeItem('nagarik_token');
  localStorage.removeItem('nagarik_user');
}

export function getStoredUser() {
  const raw = localStorage.getItem('nagarik_user');
  return raw ? JSON.parse(raw) : null;
}

export async function validateRequest(qrData) {
  const expiryIso = qrData.expiry ?? qrData.expiryIso;
  const params = new URLSearchParams({
    bankId: qrData.bankId,
    expiry: expiryIso,
    signature: qrData.signature,
  });

  const response = await fetch(
    `${API_BASE}/api/request/${qrData.requestId}?${params.toString()}`,
    { headers: authHeaders() }
  );

  const data = await handleResponse(response);
  return {
    requestId: data.requestId ?? data.RequestId,
    bankId: data.bankId ?? data.BankId,
    expiryIso: data.expiryIso ?? data.ExpiryIso ?? expiryIso,
    isValid: data.isValid ?? data.IsValid,
    message: data.message ?? data.Message,
  };
}

export async function approveConsent(payload) {
  const expiryIso = payload.expiry ?? payload.expiryIso;
  const response = await fetch(`${API_BASE}/api/consent/approve`, {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify({
      requestId: payload.requestId,
      bankId: payload.bankId,
      expiryIso,
      qrSignature: payload.qrSignature ?? payload.signature,
    }),
  });

  return handleResponse(response);
}

export async function getProfile() {
  const response = await fetch(`${API_BASE}/api/profile`, {
    headers: authHeaders(),
  });
  const data = await handleResponse(response);
  return {
    userId: data.userId ?? data.UserId,
    fullName: data.fullName ?? data.FullName,
    kycData: data.kycData ?? data.KycData,
  };
}
