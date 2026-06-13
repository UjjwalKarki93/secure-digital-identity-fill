import { useQuery } from '@tanstack/react-query';
import { getProfile } from '../services/api';

export default function Profile() {
  const { data, isLoading, error } = useQuery({
    queryKey: ['profile'],
    queryFn: getProfile,
  });

  if (isLoading) {
    return <div className="container"><div className="card">Loading profile...</div></div>;
  }

  if (error) {
    return <div className="container"><div className="card">Failed to load profile.</div></div>;
  }

  const kyc = data.kycData;

  return (
    <div className="container">
      <div className="card">
        <h2 style={{ marginBottom: '1rem' }}>My Profile</h2>
        <p style={{ marginBottom: '1rem' }}><strong>{data.fullName}</strong></p>
        <div style={{ display: 'grid', gap: '0.5rem', fontSize: '0.95rem' }}>
          <p><span style={{ color: 'var(--muted)' }}>National ID:</span> {kyc.nationalId}</p>
          <p><span style={{ color: 'var(--muted)' }}>Date of Birth:</span> {kyc.dateOfBirth}</p>
          <p><span style={{ color: 'var(--muted)' }}>Address:</span> {kyc.address}</p>
          <p><span style={{ color: 'var(--muted)' }}>Phone:</span> {kyc.phoneNumber}</p>
          <p><span style={{ color: 'var(--muted)' }}>Email:</span> {kyc.email}</p>
        </div>
      </div>
    </div>
  );
}
