# PROD-READINESS.md — E-Commerce-Portafolio

> Auditoría de producción basada en evidencia del repositorio (sin ejecutar código).
> Fecha de auditoría: 2026-09-10. Criterio: "¿un reclutador puede correrlo y evaluarlo en pocos comandos?"

---

## 1. Estado actual

**Stack real (verificado en archivos):**
- **Backend:** .NET 10 (ASP.NET Core) + Entity Framework Core + PostgreSQL (vía Supabase connection pooler `aws-0-us-east-1.pooler.supabase.com`). Entrypoint: `Backend/Backend/Program.cs`, proyecto `Backend/Backend/Backend.csproj`.
- **Frontend:** React 19 + Vite 8 + React Router 7 (`react-router-dom`). Gestor de dependencias real: **pnpm** (existe `pnpm-lock.yaml`; `npm install` del README es impreciso). Entrypoint: `Frontend/src/main.jsx`.
- **Auth:** JWT (Access + Refresh) con BCrypt, según README. `Program.cs` valida issuer/audience/lifetime y firma.
- **Deploy (según README):** Docker + FTP (Alwaysdata) + Azure DevOps.

**Hallazgos de madurez (positivos, verificados en `Program.cs`):**
- CORS con allowlist explícita (`localhost:5173`, `localhost:3000`, `https://e-commerce-portafolio.onrender.com`) — **no** `AllowAnyOrigin`.
- Rate limiting particionado por IP (`general` 120/min, `auth-strict` 10/min anti brute-force).
- `ForwardedHeaders` configurado (ForwardLimit=2) delante de HSTS.
- HSTS en producción; OpenAPI/Swagger **solo** en Development (no expuesto en prod).
- Middleware de security headers: `X-Content-Type-Options`, `X-Frame-Options: DENY`, `Referrer-Policy`, `Content-Security-Policy: default-src 'self'`, `Permissions-Policy`, COOP/COI.
- Secretos JWT leídos de variables de entorno (`JWT_SECRET`, `JWT_ISSUER`, `JWT_AUDIENCE`), no hardcoded.

**Estado de producción (resumen):**
| Ítem | Estado |
|------|--------|
| Autenticación JWT | ✅ Implementada (env-first) |
| Migraciones EF | ⚠️ **No hay carpeta `Migrations/`**; solo `Backend/Backend/migration.sql` manual (169 líneas) |
| Tests | ❌ No existe proyecto de tests (README confirma "pendientes") |
| Logging | ⚠️ Solo `appsettings` default (Information); sin sink estructurado verificado |
| CORS | ✅ Allowlist |
| Swagger en prod | ✅ No expuesto |
| Pipeline CI/CD | ❌ **No existe** `azure-pipelines.yml` ni `.github/` en el repo (README lo afirma pero no está) |
| Docker | ⚠️ Solo backend (`Backend/Dockerfile`); no hay `docker-compose` |

---

## 2. Tabla priorizada — qué falta (P0 / P1 / P2)

| Pri | Falta / Bloqueo | Dónde colocar / archivo |
|-----|-----------------|--------------------------|
| **P0** | **Migraciones EF ausentes**: `Data/` solo tiene `AppDbContext.cs`. `dotnet ef database update` fallará. Schema debe crearse manualmente con `migration.sql`. | Generar `Backend/Backend/Data/Migrations/` (`dotnet ef migrations add Initial`) o documentar `psql -f migration.sql` en README/CI |
| **P0** | **Pipeline CI/CD inexistente** pese a afirmarse en README. Un reclutador no tiene forma automatizada de build/deploy. | Crear `azure-pipelines.yml` (build backend+frontend, test, deploy FTP/Vercel) en raíz |
| **P0** | **Frontend no apunta al backend**: grep de `src/` no halló ninguna URL/base de API (integración "en progreso" según README). El recruiter ve UI sin datos. | `Frontend/src/services/*` o `vite.config.js` proxy `/api` → definir `VITE_API_BASE_URL` |
| **P1** | **Tests ausentes** (0 cobertura). | Crear `Backend/Backend.Tests/` (xUnit) + tests frontend (vitest) |
| **P1** | **Frontend usa `npm` en README pero el lockfile es pnpm** → `npm install` puede divergir. | README: usar `pnpm install` / `pnpm dev` |
| **P1** | **Deploy frontend desdocumentado**: README dice "FTP / Vercel / Netlify" sin script ni `vercel.json`/`Dockerfile`. | Añadir `Frontend/vercel.json` o paso en pipeline |
| **P2** | **`appsettings.json` con host/usuario Supabase reales** (password `CHANGE_ME`). Está en `.gitignore` → no se sube, pero queda en disco local. | Mantener fuera del repo; usar solo `DATABASE_URL` por env |
| **P2** | **`README` muestra JWT `Key` hardcoded de ejemplo** (obsoleto vs `Program.cs` env-first). Confunde. | Actualizar sección "Variables de Entorno" del README |

---

## 3. Variables de entorno

**Backend (obligatorias para arrancar — `Program.cs` hace throw si faltan):**
| Variable | Descripción | Ejemplo / fuente |
|----------|-------------|------------------|
| `DATABASE_URL` | Connection string PostgreSQL (tiene prioridad sobre `appsettings`) | `Host=...;Database=...;Username=...;Password=...;SSL Mode=Require` |
| `JWT_SECRET` | Clave HMAC (obligatoria, falla si ausente) | 32+ chars aleatorios |
| `JWT_ISSUER` | Emisor JWT (default `api-comidas`) | `ECommercePortafolio` |
| `JWT_AUDIENCE` | Audiencia JWT (default `app-comidas`) | `ECommercePortafolioUsers` |

> Fallback: si no hay `DATABASE_URL`, usa `ConnectionStrings:DefaultConnection` de `appsettings.json`/`Development.json` (este último gitignoreado).

**Frontend:**
- No se encontró `.env` ni constante de base URL en `src/` → **no verificado**. Recomendado: `VITE_API_BASE_URL=http://localhost:5173` (o el origen CORS permitido).

---

## 4. Plan deployment (comandos)

**Local (desarrollo) — backend:**
```bash
cd Backend
dotnet restore
# Crear/actualizar schema (ver P0): no usar 'dotnet ef database update' sin Migrations
psql "$DATABASE_URL" -f Backend/Backend/migration.sql
$env:JWT_SECRET="<32+ chars>"; $env:DATABASE_URL="<postgres>"
dotnet run --project Backend/Backend/Backend.csproj
```

**Local — frontend:**
```bash
cd Frontend
pnpm install        # NO npm install (lockfile es pnpm)
pnpm dev            # http://localhost:5173
```

**Contenedor (solo backend, existe Dockerfile multi-etapa):**
```bash
docker build -t ecommerce-backend ./Backend
docker run -e DATABASE_URL=... -e JWT_SECRET=... -p 8080:8080 ecommerce-backend
```

**Producción (no verificado — no hay pipeline en repo):**
- Backend: Docker → Alwaysdata vía FTP (script **no presente** en repo).
- Frontend: `pnpm build` → `dist/` → FTP / Vercel / Netlify (sin config confirmada).
- CI/CD: Azure DevOps **no configurado en este repo** (crear `azure-pipelines.yml`).

> **Blocker "2 comandos":** No es viable hoy. Requiere (1) PostgreSQL accesible + `DATABASE_URL`, (2) aplicar `migration.sql`, (3) `JWT_SECRET`, (4) build frontend con URL de API. Mínimo realista: ~4 pasos + secretos.

---

## 5. Riesgos de seguridad

| Riesgo | Severidad | Evidencia |
|--------|-----------|-----------|
| `appsettings.json` contiene host/usuario Supabase reales | Baja (gitignoreado) | `Backend/Backend/appsettings.json` línea 10 (password `CHANGE_ME`) |
| JWT hardcoded en ejemplo de README | Baja (documentación) | `README.md` líneas 75-81 |
| CSP `default-src 'self'` estricto puede romper assets externos (Google/FB login) | Media (funcional) | `Program.cs` middleware CSP |
| Sin tests de seguridad / auth | Media | Sin proyecto de tests |
| Frontend sin base URL de API definida | Media (disponibilidad) | grep `src/` sin coincidencias |
| Secretos en código | **No encontrado** | `Program.cs` usa env vars; grep sin secretos |

**Positivo:** CORS allowlist, rate limiting, HSTS, security headers y env-first JWT están correctamente implementados — es el repo más maduro en seguridad de los tres auditados.

---

## 6. Notas

- No se modificó ningún archivo del repositorio; este documento es de solo lectura/auditoría.
- Ítems marcados **"no verificado"** = no presentes en el repo tras inspección; requieren confirmación del autor.
