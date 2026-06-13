import { Link } from 'react-router-dom';

export default function Home() {
  return (
    <>
      <section className="hero-banner">
        <div className="container">
          <p className="tagline">Digital Banking</p>
          <h1 style={{ fontSize: '2.5rem', lineHeight: 1.2, marginBottom: '1rem', maxWidth: '640px' }}>
            Open your account with verified Nagarik digital identity
          </h1>
          <p style={{ fontSize: '1.1rem', lineHeight: 1.7, opacity: 0.9, maxWidth: '560px', marginBottom: '2rem' }}>
            Laxmi Sunrise Bank partners with Nagarik to verify your identity securely.
            Scan a QR code, approve consent, and your application form fills instantly.
          </p>
          <Link to="/open-account" className="btn btn-primary" style={{ fontSize: '1rem' }}>
            Open New Account
          </Link>
        </div>
      </section>

      <div className="container">
        <section style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(260px, 1fr))', gap: '1.25rem' }}>
          {[
            {
              title: 'Secure Verification',
              desc: 'HMAC-signed QR codes ensure every identity request is authentic and tamper-proof.',
              icon: '🔐',
            },
            {
              title: 'Nagarik Integration',
              desc: 'Verify your identity through Nepal\'s national digital ID app with one tap consent.',
              icon: '📱',
            },
            {
              title: 'Instant Auto-Fill',
              desc: 'Approved KYC data flows directly into your account application — no retyping.',
              icon: '⚡',
            },
          ].map((item) => (
            <div className="card" key={item.title}>
              <div style={{ fontSize: '1.75rem', marginBottom: '0.75rem' }}>{item.icon}</div>
              <h4 style={{ color: 'var(--navy)', marginBottom: '0.5rem' }}>{item.title}</h4>
              <p style={{ color: 'var(--muted)', fontSize: '0.95rem', lineHeight: 1.6 }}>{item.desc}</p>
            </div>
          ))}
        </section>
      </div>
    </>
  );
}
