export function parseQrPayload(rawInput) {
  const trimmed = rawInput.trim();
  if (!trimmed) {
    throw new Error('QR payload is empty.');
  }

  let parsed;
  try {
    parsed = JSON.parse(trimmed);
  } catch {
    throw new Error('Invalid QR format. Expected JSON payload.');
  }

  const required = ['requestId', 'bankId', 'signature'];
  const missing = required.filter((key) => !parsed[key]);
  if (missing.length > 0) {
    throw new Error(`QR missing fields: ${missing.join(', ')}`);
  }

  const expiryIso = parsed.expiry ?? parsed.expiryIso;
  if (!expiryIso) {
    throw new Error('QR missing expiry field.');
  }

  return {
    requestId: parsed.requestId,
    bankId: parsed.bankId,
    expiry: expiryIso,
    expiryIso,
    signature: parsed.signature,
  };
}

export function isQrExpired(expiryIso) {
  return new Date(expiryIso).getTime() < Date.now();
}

export function parseQrFromUrl(searchParams) {
  const qrParam = searchParams.get('qr');
  if (!qrParam) return null;
  try {
    const decoded = decodeURIComponent(qrParam);
    return parseQrPayload(decoded);
  } catch {
    return null;
  }
}
