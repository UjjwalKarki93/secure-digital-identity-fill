export default function Form({ formData, onChange, disabled }) {
  const fields = [
    { name: 'fullName', label: 'Full Name' },
    { name: 'nationalId', label: 'National ID' },
    { name: 'dateOfBirth', label: 'Date of Birth' },
    { name: 'address', label: 'Address' },
    { name: 'phoneNumber', label: 'Phone Number' },
    { name: 'email', label: 'Email', type: 'email' },
  ];

  return (
    <div className="card">
      <h3 style={{ marginBottom: '1rem', color: 'var(--navy)' }}>Laxmi Sunrise Account Application</h3>
      <div className="form-grid">
        {fields.map((field) => (
          <div className="form-group" key={field.name}>
            <label htmlFor={field.name}>{field.label}</label>
            <input
              id={field.name}
              name={field.name}
              type={field.type || 'text'}
              value={formData[field.name] || ''}
              onChange={(e) => onChange(field.name, e.target.value)}
              readOnly={disabled}
              placeholder={disabled ? 'Awaiting identity verification...' : `Enter ${field.label.toLowerCase()}`}
            />
          </div>
        ))}
      </div>
    </div>
  );
}
