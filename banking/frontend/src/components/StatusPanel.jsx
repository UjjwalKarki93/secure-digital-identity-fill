export default function StatusPanel({ status, error, signalRStatus }) {
  const statusMap = {
    idle: { label: 'Ready', className: 'badge-pending', message: 'Click "Verify with Nagarik" to begin identity verification.' },
    starting: { label: 'Starting', className: 'badge-pending', message: 'Generating your secure Laxmi Sunrise Bank QR code...' },
    waiting: { label: 'Waiting', className: 'badge-pending', message: 'Upload the QR screenshot in Nagarik and approve consent.' },
    expired: { label: 'Expired', className: 'badge-error', message: 'The QR code has expired. Generate a new QR to continue verification.' },
    completed: { label: 'Verified', className: 'badge-success', message: 'Identity verified! Your account application has been auto-filled.' },
    error: { label: 'Error', className: 'badge-error', message: error || 'Something went wrong.' },
  };

  const current = statusMap[status] || statusMap.idle;

  return (
    <div className="card">
      <h3 style={{ marginBottom: '0.75rem', color: 'var(--navy)' }}>Verification Status</h3>
      <span className={`badge ${current.className}`} style={{ marginBottom: '0.75rem' }}>
        {current.label}
      </span>
      <p style={{ color: 'var(--muted)', lineHeight: 1.6 }}>{current.message}</p>
      {status === 'waiting' && (
        <p style={{ color: 'var(--muted)', fontSize: '0.85rem', marginTop: '0.75rem' }}>
          Live updates via SignalR ({signalRStatus}). Polling every 3s as fallback.
        </p>
      )}
    </div>
  );
}
