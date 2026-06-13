import { useEffect, useState } from 'react';
import { QRCodeSVG } from 'qrcode.react';
import { buildQrPayload, getExpiryCountdown } from '../utils/qrGenerator';

export default function QRCodeBox({ verification, status, signalRStatus, onRegenerate }) {
  const [countdown, setCountdown] = useState({ label: '2:00', isExpired: false });

  const expiry = verification?.expiry ?? verification?.expiryIso;
  const qrValue = verification ? buildQrPayload(verification) : '';
  const showExpiredPanel = status === 'expired';
  const isCompleted = status === 'completed';
  const isRegenerating = status === 'starting' || status === 'waiting';

  useEffect(() => {
    if (!verification?.requestId) return;
    const next = getExpiryCountdown(verification.expiry ?? verification.expiryIso);
    setCountdown(next);
  }, [verification?.requestId, verification?.expiry, verification?.expiryIso]);

  useEffect(() => {
    if (!expiry || showExpiredPanel || isCompleted || !isRegenerating) return undefined;

    const update = () => {
      setCountdown(getExpiryCountdown(expiry));
    };

    update();
    const interval = setInterval(update, 1000);
    return () => clearInterval(interval);
  }, [expiry, showExpiredPanel, isCompleted, isRegenerating]);

  if (!verification || status === 'starting') {
    return (
      <div className="card qr-empty">
        <div className="qr-empty-icon">{status === 'starting' ? '⏳' : '📲'}</div>
        <p>
          {status === 'starting'
            ? 'Generating your secure QR code...'
            : <>Click <strong>Verify with Nagarik</strong> to generate your secure QR code.</>}
        </p>
      </div>
    );
  }

  if (showExpiredPanel && !isCompleted) {
    return (
      <div className="card qr-expired-panel">
        <div className="qr-expired-icon">⏱️</div>
        <h3>QR Code Expired</h3>
        <p>
          This verification QR is no longer valid for security reasons.
          Please generate a new one from the bank portal.
        </p>
        <button type="button" className="btn btn-primary" onClick={onRegenerate}>
          Generate New QR
        </button>
      </div>
    );
  }

  return (
    <div className="card" style={{ textAlign: 'center' }}>
      <h3 style={{ marginBottom: '0.25rem', color: 'var(--navy)' }}>Scan with Nagarik App</h3>
      <p style={{ color: 'var(--muted)', fontSize: '0.85rem', marginBottom: '1rem' }}>
        Upload this QR screenshot in the Nagarik app
      </p>
      <div className="qr-active-frame">
        <QRCodeSVG value={qrValue} size={220} level="H" includeMargin />
      </div>
      <p style={{ color: 'var(--muted)', fontSize: '0.85rem', marginBottom: '0.5rem' }}>
        Request ID: <code style={{ color: 'var(--navy)' }}>{verification.requestId}</code>
      </p>
      <p className="qr-countdown">
        Valid for: <strong>{countdown.label}</strong>
      </p>
      <p style={{ color: 'var(--muted)', fontSize: '0.8rem', marginBottom: '0.75rem' }}>
        Live status: {signalRStatus === 'connected' ? '🟢 Connected' : signalRStatus === 'reconnecting' ? '🟡 Reconnecting' : '⚪ Waiting'}
      </p>
      <span className={`badge ${isCompleted ? 'badge-success' : 'badge-pending'}`}>
        {isCompleted ? 'Identity Verified' : 'Awaiting Upload'}
      </span>
    </div>
  );
}
