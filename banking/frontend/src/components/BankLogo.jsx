export default function BankLogo({ size = 'md' }) {
  const isLarge = size === 'lg';
  const iconSize = isLarge ? 48 : 36;

  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: isLarge ? '0.75rem' : '0.5rem' }}>
      <svg width={iconSize} height={iconSize} viewBox="0 0 48 48" aria-hidden="true">
        <circle cx="24" cy="24" r="22" fill="#F58220" />
        <path
          d="M24 10 L28 20 L38 22 L30 30 L32 40 L24 34 L16 40 L18 30 L10 22 L20 20 Z"
          fill="#fff"
          opacity="0.95"
        />
        <circle cx="24" cy="24" r="6" fill="#003B71" />
      </svg>
      <div>
        <div style={{
          fontWeight: 700,
          fontSize: isLarge ? '1.5rem' : '1.1rem',
          color: 'var(--navy)',
          lineHeight: 1.1,
        }}>
          Laxmi Sunrise Bank
        </div>
        {!isLarge && (
          <div style={{ fontSize: '0.7rem', color: 'var(--orange)', fontWeight: 600, letterSpacing: '0.04em' }}>
            Aspire. Together.
          </div>
        )}
      </div>
    </div>
  );
}
