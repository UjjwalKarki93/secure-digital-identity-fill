const API_BASE = import.meta.env.VITE_BANKING_API_URL ?? '';

async function handleResponse(response) {
  if (!response.ok) {
    const error = await response.json().catch(() => ({}));
    const message = error.errors?.[0] || error.message || `Request failed (${response.status})`;
    throw new Error(message);
  }
  return response.json();
}

export function normalizeVerificationResponse(data) {
  return {
    requestId: data.requestId ?? data.RequestId,
    bankId: data.bankId ?? data.BankId,
    expiry: data.expiry ?? data.Expiry ?? data.expiryIso ?? data.ExpiryIso,
    signature: data.signature ?? data.Signature,
    qrPayload: data.qrPayload ?? data.QrPayload,
  };
}

export async function startVerification(bankId = 'BANK-001') {
  const response = await fetch(`${API_BASE}/api/verification/start`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ bankId }),
  });
  const data = await handleResponse(response);
  return normalizeVerificationResponse(data);
}

export async function getVerificationStatus(requestId) {
  const response = await fetch(`${API_BASE}/api/verification/${requestId}`);
  const data = await handleResponse(response);
  return {
    requestId: data.requestId ?? data.RequestId,
    status: data.status ?? data.Status,
    expiresAt: data.expiresAt ?? data.ExpiresAt,
    completedAt: data.completedAt ?? data.CompletedAt,
    identityData: data.identityData ?? data.IdentityData ?? null,
  };
}
