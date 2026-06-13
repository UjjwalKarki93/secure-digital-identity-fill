import { useState } from 'react';
import ConsentModal from '../components/ConsentModal';
import QRUploader from '../components/QRUploader';
import { useAuth } from '../hooks/useAuth';
import { approveConsent, validateRequest } from '../services/api';
import { isQrExpired, parseQrPayload } from '../utils/qrParser';

export default function Scan() {
  const { user, isAuthenticated, login, logout } = useAuth();
  const [username, setUsername] = useState('citizen');
  const [password, setPassword] = useState('nagarik123');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [approving, setApproving] = useState(false);
  const [qrData, setQrData] = useState(null);
  const [showConsent, setShowConsent] = useState(false);
  const [success, setSuccess] = useState(false);

  const handleLogin = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess(false);
    setLoading(true);
    try {
      await login(username, password);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleQrUpload = async (rawText) => {
    if (!isAuthenticated) {
      setError('Please log in before uploading a QR code.');
      return;
    }

    setError('');
    setSuccess(false);
    setLoading(true);

    try {
      const parsed = parseQrPayload(rawText);

      if (isQrExpired(parsed.expiryIso ?? parsed.expiry)) {
        setError('This QR code has expired. Go back to Laxmi Sunrise Bank and generate a new QR.');
        return;
      }

      const validation = await validateRequest(parsed);

      if (!validation.isValid) {
        setError(validation.message);
        return;
      }

      setQrData(parsed);
      setShowConsent(true);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleDecline = () => {
    setShowConsent(false);
    setQrData(null);
  };

  const handleApprove = async () => {
    if (!qrData) return;

    setApproving(true);
    setError('');

    try {
      if (isQrExpired(qrData.expiryIso ?? qrData.expiry)) {
        setError('This QR code expired before consent could be shared. Please upload a new QR from the bank.');
        setShowConsent(false);
        setQrData(null);
        return;
      }

      await approveConsent({
        requestId: qrData.requestId,
        bankId: qrData.bankId,
        expiryIso: qrData.expiryIso ?? qrData.expiry,
        qrSignature: qrData.signature,
      });
      setShowConsent(false);
      setQrData(null);
      setSuccess(true);
    } catch (err) {
      setError(err.message);
    } finally {
      setApproving(false);
    }
  };

  return (
    <div className="container">
      {!isAuthenticated ? (
        <div className="card" style={{ marginBottom: '1rem' }}>
          <h2 style={{ marginBottom: '1rem' }}>Login</h2>
          <form onSubmit={handleLogin}>
            <input
              placeholder="Username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
            <input
              type="password"
              placeholder="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
            <button type="submit" className="btn btn-primary" disabled={loading}>
              {loading ? 'Signing in...' : 'Sign In'}
            </button>
          </form>
          <p className="demo-hint">Demo: citizen / nagarik123</p>
        </div>
      ) : (
        <div className="card" style={{ marginBottom: '1rem' }}>
          <p>Hello, <strong>{user.fullName}</strong></p>
          <button type="button" className="btn btn-secondary" style={{ marginTop: '0.75rem' }} onClick={logout}>
            Logout
          </button>
        </div>
      )}

      {success && (
        <div className="success-banner">
          Identity shared! The bank application form will auto-fill now.
        </div>
      )}

      <div className="card">
        <h2 style={{ marginBottom: '0.5rem' }}>Verify with Bank QR</h2>
        <p style={{ color: 'var(--muted)', fontSize: '0.9rem', marginBottom: '1rem' }}>
          Upload the QR image from Laxmi Sunrise Bank account opening page.
        </p>

        <QRUploader
          onUpload={handleQrUpload}
          onError={setError}
          disabled={!isAuthenticated}
          loading={loading}
        />
      </div>

      {error && <p className="scan-error">{error}</p>}

      <ConsentModal
        open={showConsent}
        qrData={qrData}
        approving={approving}
        onApprove={handleApprove}
        onDecline={handleDecline}
      />
    </div>
  );
}
