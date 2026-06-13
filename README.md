# Secure Digital Identity Verification System

Production-grade dual-system architecture for bank identity verification via HMAC-signed QR codes, JWT-authenticated consent, and real-time SignalR form auto-fill.


## Architecture

```
Banking Portal (React :3000) ──► Banking API (:5001) ──► PostgreSQL (banking_db)
                                      ▲
                                      │ HMAC webhook
Nagarik App (React :3001) ──► Identity API (:5002) ──► PostgreSQL (identity_db)
```

## Tech Stack

| Layer | Banking | Identity |
|-------|---------|----------|
| Frontend | React, React Router, React Query, SignalR, qrcode.react | React, React Router, React Query |
| Backend | ASP.NET Core 8, Clean Architecture, MediatR, FluentValidation, EF Core, SignalR | ASP.NET Core 8, Clean Architecture, MediatR, FluentValidation, EF Core, JWT |
| Database | PostgreSQL | PostgreSQL |
| Security | HMAC SHA256, rate limiting, audit logs | HMAC SHA256, JWT, rate limiting, audit logs |

## Quick Start (Docker — recommended for demo)

**One command runs everything** (Postgres, both APIs, both frontends, migrations + seed data):

```bash
docker compose up --build
```

Or on Windows:

```powershell
.\start-dev.ps1
```

| Service | URL |
|---------|-----|
| Bank Portal | http://localhost:3000 |
| Nagarik App | http://localhost:3001 |
| Banking API Swagger | http://localhost:5001/swagger |
| Identity API Swagger | http://localhost:5002/swagger |

**Demo logins** (seeded automatically):

| Username | Password | Name |
|----------|----------|------|
| `citizen` | `nagarik123` | Ram Bahadur Thapa |
| `sita` | `nagarik123` | Sita Devi Sharma |

Stop the demo: `docker compose down`

---

## Local Development (without Docker)

### 1. Start PostgreSQL

```bash
docker compose up -d postgres
```

Create databases (first run only):

```sql
CREATE DATABASE banking_db;
CREATE DATABASE identity_db;
```

### 2. Run Banking API

```bash
cd banking/src/Banking.API
dotnet run
```

Swagger: http://localhost:5001/swagger

### 3. Run Identity API

```bash
cd identity/src/Identity.API
dotnet run
```

Swagger: http://localhost:5002/swagger

### 4. Run Frontends

Frontends use **Vite dev proxies** — API calls go to `/api` and SignalR to `/hubs` (no CORS setup needed).

```bash
cd banking/frontend && npm install && npm run dev   # http://localhost:3000
cd identity/frontend && npm install && npm run dev  # http://localhost:3001
```

**Quick start (Windows):** run `.\start-dev.ps1` to launch PostgreSQL + both APIs.

### Integrated Flow (UI)

1. Bank Portal → **Open Account** → **Verify Identity** (calls `POST /api/verification/start`, connects SignalR)
2. Click **Open in Nagarik App** (deep-links to Nagarik with QR data) or **Copy QR JSON**
3. Nagarik App → login → QR auto-filled from link → **Validate & Continue** (calls `GET /api/request/{id}`)
4. **Approve & Share Identity** (calls `POST /api/consent/approve` → webhook → SignalR → form auto-fill)

## Demo Credentials

| System | Credential |
|--------|------------|
| Nagarik App | `citizen` / `nagarik123` |
| Bank ID | `BANK-001` |

## End-to-End Flow

1. **Bank Portal** → `POST /api/verification/start` → generates HMAC-signed QR (2 min expiry)
2. **React** connects to SignalR hub, joins `requestId` group
3. **Nagarik App** scans QR → `GET /api/request/{requestId}` validates signature
4. User approves consent → `POST /api/consent/approve`
5. **Identity API** signs webhook payload → `POST /api/identity/webhook`
6. **Banking API** validates HMAC, marks request used, pushes `VerificationCompleted` via SignalR
7. **Bank Portal** auto-fills the account form

## API Reference

### Banking API (`:5001`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/verification/start` | Start verification, return QR payload |
| GET | `/api/verification/{requestId}` | Get verification status |
| POST | `/api/identity/webhook` | Receive identity callback |
| WS | `/hubs/verification` | SignalR hub |

### Identity API (`:5002`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/login` | No | Mock login, returns JWT |
| GET | `/api/request/{requestId}` | JWT | Validate QR request |
| POST | `/api/consent/approve` | JWT | Approve and send webhook |
| GET | `/api/profile` | JWT | Get user KYC profile |

## Example Requests

### Start Verification

```http
POST /api/verification/start
Content-Type: application/json

{ "bankId": "BANK-001" }
```

```json
{
  "requestId": "a1b2c3d4-...",
  "bankId": "BANK-001",
  "expiry": "2026-06-12T10:02:00Z",
  "signature": "base64-hmac...",
  "qrPayload": "{\"requestId\":\"...\",\"bankId\":\"BANK-001\",\"expiry\":\"...\",\"signature\":\"...\"}"
}
```

### QR Payload (embedded in QR code)

```json
{
  "requestId": "guid",
  "bankId": "BANK-001",
  "expiry": "2026-06-12T10:02:00Z",
  "signature": "HMAC-SHA256-base64"
}
```

**Signing:** `HMAC_SHA256("{requestId}|{bankId}|{expiry:O}", sharedSecret)`

### Webhook Callback

```http
POST /api/identity/webhook
Content-Type: application/json

{
  "requestId": "guid",
  "userId": "user-guid",
  "identityData": {
    "fullName": "Ram Bahadur Thapa",
    "nationalId": "123-456-78901",
    "dateOfBirth": "1990-05-15",
    "address": "Kathmandu, Nepal",
    "phoneNumber": "+977-9841234567",
    "email": "ram.thapa@example.com"
  },
  "timestamp": "2026-06-12T10:01:30Z",
  "signature": "base64-hmac..."
}
```

**Webhook signing:** `HMAC_SHA256("{requestId}|{userId}|{timestamp:O}|{identityJson}", sharedSecret)`

## Security Features

- HMAC SHA256 on QR payloads and webhooks
- JWT authentication (Identity API only)
- 2-minute request expiry
- One-time `requestId` (replay prevention)
- Audit logging on all verification actions
- FluentValidation on all commands/queries
- Rate limiting on sensitive endpoints
- HTTPS/HSTS in production

## Project Structure

```
banking/src/
├── Banking.Domain/
├── Banking.Application/     # CQRS (MediatR), DTOs, Validators
├── Banking.Infrastructure/  # EF Core, Repositories, HMAC
└── Banking.API/             # Controllers, SignalR Hub, Middleware

identity/src/
├── Identity.Domain/
├── Identity.Application/
├── Identity.Infrastructure/
└── Identity.API/
```

## Configuration

Shared HMAC secret (must match in both APIs):

```json
"Security": { "HmacSecretKey": "shared-hmac-secret-key-change-in-production" }
```

Change all secrets before production deployment.
