import { useId, useRef } from 'react';
import { Html5Qrcode } from 'html5-qrcode';

export default function QRUploader({ onUpload, onError, disabled, loading }) {
  const inputId = useId().replace(/:/g, '');
  const fileInputRef = useRef(null);
  const scannerRef = useRef(null);

  const handleFileChange = async (event) => {
    const file = event.target.files?.[0];
    if (!file || disabled || loading) return;

    const scanner = new Html5Qrcode(`qr-upload-${inputId}`, false);
    scannerRef.current = scanner;

    try {
      const decodedText = await scanner.scanFile(file, false);
      onUpload(decodedText);
    } catch {
      onError('Could not read QR code from this image. Please upload a clear screenshot of the bank QR.');
    } finally {
      if (fileInputRef.current) {
        fileInputRef.current.value = '';
      }
      try {
        scanner.clear();
      } catch {
        // ignore
      }
      scannerRef.current = null;
    }
  };

  return (
    <div className="qr-uploader">
      <div id={`qr-upload-${inputId}`} style={{ display: 'none' }} aria-hidden="true" />
      <div className="qr-upload-zone">
        <div className="qr-upload-icon">🖼️</div>
        <p className="qr-upload-title">Upload Bank QR Image</p>
        <p className="qr-upload-hint">
          Take a screenshot of the QR from Laxmi Sunrise Bank and upload it here.
        </p>
        <input
          ref={fileInputRef}
          id={`qr-file-${inputId}`}
          type="file"
          accept="image/*"
          className="qr-file-input"
          onChange={handleFileChange}
          disabled={disabled || loading}
        />
        <label
          htmlFor={`qr-file-${inputId}`}
          className={`btn btn-primary qr-upload-btn ${disabled || loading ? 'disabled' : ''}`}
        >
          {loading ? 'Processing...' : 'Choose QR Image'}
        </label>
      </div>
    </div>
  );
}
