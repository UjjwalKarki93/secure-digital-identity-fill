export function buildQrPayload(verification) {
  if (verification?.qrPayload) {
    return verification.qrPayload;
  }

  const expiry = verification.expiry ?? verification.expiryIso;
  return JSON.stringify({
    requestId: verification.requestId,
    bankId: verification.bankId,
    expiry,
    signature: verification.signature,
  });
}

export function isQrExpired(expiryIso) {
  if (!expiryIso) return false;
  return new Date(expiryIso).getTime() <= Date.now();
}

export function getExpiryCountdown(expiryIso) {
  const expiryMs = new Date(expiryIso).getTime();
  const remaining = Math.max(0, Math.floor((expiryMs - Date.now()) / 1000));
  const minutes = Math.floor(remaining / 60);
  const seconds = remaining % 60;
  return {
    remaining,
    isExpired: remaining <= 0,
    label: remaining <= 0 ? 'Expired' : `${minutes}:${seconds.toString().padStart(2, '0')}`,
  };
}

export function getNagarikDeepLink(qrPayload) {
  const nagarikUrl = import.meta.env.VITE_NAGARIK_APP_URL || 'http://localhost:3001';
  const encoded = encodeURIComponent(qrPayload);
  return `${nagarikUrl}/scan?qr=${encoded}`;
}
