import { useCallback, useEffect, useRef, useState } from 'react';
import { startVerification, getVerificationStatus } from '../services/api';
import { createVerificationConnection } from '../services/signalr';
import { isQrExpired } from '../utils/qrGenerator';

const POLL_INTERVAL_MS = 3000;

export function useVerification() {
  const [verification, setVerification] = useState(null);
  const [status, setStatus] = useState('idle');
  const [identityData, setIdentityData] = useState(null);
  const [error, setError] = useState(null);
  const [signalRStatus, setSignalRStatus] = useState('disconnected');
  const connectionRef = useRef(null);
  const pollRef = useRef(null);
  const activeRequestIdRef = useRef(null);

  const stopPolling = useCallback(() => {
    if (pollRef.current) {
      clearInterval(pollRef.current);
      pollRef.current = null;
    }
  }, []);

  const cleanupConnection = useCallback(async () => {
    stopPolling();
    if (connectionRef.current) {
      await connectionRef.current.stop();
      connectionRef.current = null;
    }
    setSignalRStatus('disconnected');
  }, [stopPolling]);

  const markExpired = useCallback(async (requestId) => {
    if (requestId && activeRequestIdRef.current !== requestId) return;
    await cleanupConnection();
    setStatus('expired');
    setError(null);
  }, [cleanupConnection]);

  const applyIdentityData = useCallback((data) => {
    if (!data) return;
    setIdentityData(data);
    setStatus('completed');
    stopPolling();
  }, [stopPolling]);

  const pollStatus = useCallback(async (requestId) => {
    try {
      const result = await getVerificationStatus(requestId);
      if (result.identityData) {
        applyIdentityData(result.identityData);
      } else if (result.status === 'Expired') {
        await markExpired(requestId);
      }
    } catch {
      // Polling errors are non-fatal; SignalR may still deliver the result.
    }
  }, [applyIdentityData, markExpired]);

  const startPolling = useCallback((requestId) => {
    stopPolling();
    pollRef.current = setInterval(() => pollStatus(requestId), POLL_INTERVAL_MS);
  }, [pollStatus, stopPolling]);

  const start = useCallback(async (bankId = 'BANK-001') => {
    setError(null);
    activeRequestIdRef.current = null;
    setStatus('starting');
    setIdentityData(null);

    try {
      await cleanupConnection();
      const result = await startVerification(bankId);
      activeRequestIdRef.current = result.requestId;
      setVerification(result);
      setStatus('waiting');

      const hub = createVerificationConnection(result.requestId, {
        onCompleted: (payload) => applyIdentityData(payload.identityData),
        onReconnecting: () => setSignalRStatus('reconnecting'),
        onConnected: () => setSignalRStatus('connected'),
      });

      connectionRef.current = hub;
      await hub.start();
      startPolling(result.requestId);
    } catch (err) {
      setError(err.message);
      setStatus('error');
    }
  }, [applyIdentityData, cleanupConnection, startPolling]);

  useEffect(() => {
    if (status !== 'waiting' || !verification) return undefined;

    const expiry = verification.expiry ?? verification.expiryIso;
    if (!expiry) return undefined;

    const requestId = verification.requestId;
    const tick = () => {
      if (activeRequestIdRef.current !== requestId) return;
      if (isQrExpired(expiry)) {
        markExpired(requestId);
      }
    };

    tick();
    const interval = setInterval(tick, 1000);
    return () => clearInterval(interval);
  }, [verification, status, markExpired]);

  useEffect(() => () => {
    cleanupConnection();
  }, [cleanupConnection]);

  return {
    verification,
    status,
    identityData,
    error,
    signalRStatus,
    isExpired: status === 'expired',
    start,
  };
}
