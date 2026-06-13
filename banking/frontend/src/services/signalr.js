import * as signalR from '@microsoft/signalr';

const HUB_URL = import.meta.env.VITE_SIGNALR_URL || '/hubs/verification';

export function createVerificationConnection(requestId, { onCompleted, onReconnecting, onConnected }) {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(HUB_URL)
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .configureLogging(signalR.LogLevel.Information)
    .build();

  connection.on('VerificationCompleted', (payload) => {
    const identityData = payload.identityData ?? payload.IdentityData;
    onCompleted({ ...payload, identityData });
  });

  connection.onreconnecting(() => onReconnecting?.());
  connection.onreconnected(() => {
    connection.invoke('JoinVerificationGroup', String(requestId)).catch(console.error);
    onConnected?.();
  });

  const start = async () => {
    if (connection.state === signalR.HubConnectionState.Disconnected) {
      await connection.start();
      await connection.invoke('JoinVerificationGroup', String(requestId));
      onConnected?.();
    }
  };

  const stop = async () => {
    if (connection.state === signalR.HubConnectionState.Connected) {
      await connection.invoke('LeaveVerificationGroup', String(requestId));
      await connection.stop();
    }
  };

  return { connection, start, stop };
}
