# ASP.NET Core 8 Multi-Tenant Demo (Microsoft Entra ID)

Diese Beispiel-App ist eine **Razor Pages** Web App mit:

- öffentlicher Index-Seite (`/`) für Tenant-Onboarding + InMemory CRUD
- geschützter Seite (`/test`) mit OIDC Login
- Admin-Consent Callback (`/consent/callback`)

## App Registration Settings

Für die App-Registrierung in Microsoft Entra ID:

- **Supported account types**: `Accounts in any organizational directory`
- Redirect URIs:
  - `https://localhost:xxxx/signin-oidc`
  - `https://localhost:xxxx/consent/callback`

`ClientId` liegt in `appsettings.json`.
`ClientSecret` nicht hardcoden – bitte via User-Secrets oder Environment Variable setzen.

## Onboarding Ablauf

1. Tenant auf `/` eintragen (`TenantId`, `CustomerName`)
2. Pro Tenant auf **Admin Consent** klicken (muss Tenant Admin durchführen)
3. Danach mit einem User aus diesem Tenant auf `/test` anmelden

## Wichtige Hinweise

- Tenant-Allowlist basiert auf der InMemory Tenantliste.
- Wenn ein Login-Tenant nicht in der Liste ist, wird Login abgewiesen (`tenant_not_onboarded`).
- InMemory Store ist nur Demo: Daten gehen bei Neustart verloren.
