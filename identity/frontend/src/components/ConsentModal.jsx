export default function ConsentModal({
  open,
  qrData,
  approving,
  onApprove,
  onDecline,
}) {
  if (!open || !qrData) return null;

  return (
    <div className="modal-overlay" role="dialog" aria-modal="true" aria-labelledby="consent-title">
      <div className="modal-card">
        <h2 id="consent-title" className="modal-title">Consent Required</h2>
        <p className="modal-text">
          <strong>Laxmi Sunrise Bank</strong> wants to verify your identity and share
          your KYC details (name, national ID, address, phone, email) to pre-fill your
          account application.
        </p>

        <div className="modal-details">
          <p><span>Bank</span> {qrData.bankId}</p>
          <p><span>Request</span> {String(qrData.requestId).slice(0, 8)}...</p>
        </div>

        <div className="modal-actions">
          <button
            type="button"
            className="btn btn-secondary modal-btn"
            onClick={onDecline}
            disabled={approving}
          >
            Decline
          </button>
          <button
            type="button"
            className="btn btn-primary modal-btn"
            onClick={onApprove}
            disabled={approving}
          >
            {approving ? 'Sharing...' : 'Approve & Share'}
          </button>
        </div>
      </div>
    </div>
  );
}
