import { useEffect, useState } from 'react';
import QRCodeBox from '../components/QRCodeBox';
import Form from '../components/Form';
import StatusPanel from '../components/StatusPanel';
import { useVerification } from '../hooks/useVerification';
import { isFormComplete } from '../utils/validation';

const emptyForm = {
  fullName: '',
  nationalId: '',
  dateOfBirth: '',
  address: '',
  phoneNumber: '',
  email: '',
};

export default function OpenAccount() {
  const { verification, status, identityData, error, signalRStatus, isExpired, start } = useVerification();
  const [formData, setFormData] = useState(emptyForm);

  useEffect(() => {
    if (identityData) {
      setFormData({
        fullName: identityData.fullName || '',
        nationalId: identityData.nationalId || '',
        dateOfBirth: identityData.dateOfBirth || '',
        address: identityData.address || '',
        phoneNumber: identityData.phoneNumber || '',
        email: identityData.email || '',
      });
    }
  }, [identityData]);

  const handleChange = (name, value) => {
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const canSubmit = status === 'completed' && isFormComplete(formData);

  return (
    <div className="container">
      <div style={{
        background: 'var(--orange-light)',
        borderLeft: '4px solid var(--orange)',
        padding: '1rem 1.25rem',
        borderRadius: '8px',
        marginBottom: '2rem',
      }}>
        <h1 style={{ fontSize: '1.75rem', color: 'var(--navy)', marginBottom: '0.35rem' }}>
          Account Opening
        </h1>
        <p style={{ color: 'var(--muted)' }}>
          Verify your identity via Nagarik QR to auto-fill your Laxmi Sunrise Bank application.
        </p>
      </div>

      <div style={{ display: 'flex', gap: '1rem', marginBottom: '1.5rem', flexWrap: 'wrap' }}>
        <button
          type="button"
          className="btn btn-primary"
          onClick={() => start('BANK-001')}
          disabled={status === 'starting' || status === 'waiting'}
        >
          {status === 'waiting'
            ? 'Verification In Progress...'
            : isExpired
              ? 'Generate New QR'
              : 'Verify with Nagarik'}
        </button>
        {(status === 'waiting' || isExpired) && status !== 'completed' && (
          <button type="button" className="btn btn-secondary" onClick={() => start('BANK-001')}>
            {isExpired ? 'Generate New QR' : 'Regenerate QR'}
          </button>
        )}
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))', gap: '1.5rem' }}>
        <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
          <QRCodeBox
            verification={verification}
            status={status}
            signalRStatus={signalRStatus}
            onRegenerate={() => start('BANK-001')}
          />
          <StatusPanel status={status} error={error} signalRStatus={signalRStatus} />
        </div>
        <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
          <Form
            formData={formData}
            onChange={handleChange}
            disabled={status !== 'completed'}
          />
          <button
            type="button"
            className="btn btn-navy"
            disabled={!canSubmit}
            onClick={() => alert('Your Laxmi Sunrise Bank account application has been submitted successfully!')}
          >
            Submit Application
          </button>
        </div>
      </div>
    </div>
  );
}
