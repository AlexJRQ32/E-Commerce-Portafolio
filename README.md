# E-Commerce-Portafolio

E-commerce de comida (food delivery/marketplace) desarrollado como proyecto de portafolio.

## Stack Tecnológico

| Capa | Tecnología |
|------|------------|
| **Backend** | .NET 10 (ASP.NET Core), Entity Framework Core, PostgreSQL |
| **Frontend** | React 19, Vite, React Router, CSS Modules |
| **Auth** | JWT (Access + Refresh Tokens), BCrypt |
| **Deploy** | Docker, FTP (Alwaysdata), Azure DevOps |

## Estructura del Repositorio

```
E-Commerce-Portafolio/
├── Backend/          # API REST .NET (antes API-Comidas)
│   ├── Controllers/  # Auth, Users, Restaurants, Dishes, Orders, Coupons
│   ├── Models/       # Entidades, DTOs, Enums
│   ├── Data/         # AppDbContext, Migraciones EF Core
│   ├── Properties/   # launchSettings, PublishProfiles
│   └── Program.cs    # Entry point, DI, Middleware pipeline
├── Frontend/         # SPA React (antes app-comidas)
│   ├── src/
│   │   ├── components/   # UI reutilizable (SideBar, Modals, etc.)
│   │   ├── context/      # React Context (Auth, Modal)
│   │   ├── hooks/        # Custom hooks (useModal, useMappedObjects)
│   │   ├── services/     # API endpoints por dominio
│   │   ├── mocks/        # Datos de prueba para desarrollo
│   │   ├── assets/       # Imágenes, logos
│   │   ├── App.jsx       # Rutas principales
│   │   └── main.jsx      # Entry point
│   ├── public/
│   ├── index.html
│   ├── package.json
│   └── vite.config.js
��── README.md
```

## Funcionalidades Principales

- **Autenticación**: Registro/Login (email+password, Google, Facebook), JWT con refresh tokens, roles (Cliente, Dueño, Paseador)
- **Restaurantes**: CRUD, categorías, gestión de platos
- **Pedidos**: Carrito, checkout, métodos de pago, cupones de descuento
- **Perfil de usuario**: Direcciones, historial de pedidos, favoritos
- **Panel de dueño**: Gestión de restaurante, platos, pedidos recibidos, estadísticas

## Desarrollo Local

### Backend
```bash
cd Backend
dotnet restore
dotnet ef database update  # Requiere PostgreSQL local o connection string en appsettings.Development.json
dotnet run
```
API disponible en `https://localhost:7xxx` (ver launchSettings.json)

### Frontend
```bash
cd Frontend
npm install
npm run dev
```
App disponible en `http://localhost:5173`

## Variables de Entorno (Backend)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ECommercePortafolio;Username=postgres;Password=xxx"
  },
  "Jwt": {
    "Key": "clave-secreta-muy-larga-y-segura",
    "Issuer": "ECommercePortafolio",
    "Audience": "ECommercePortafolioUsers",
    "AccessTokenMinutes": 15,
    "RefreshTokenDays": 7
  },
  "FrontendUrl": "http://localhost:5173"
}
```

## Despliegue

- **Backend**: Docker → Alwaysdata (FTP) / Azure DevOps pipeline
- **Frontend**: `npm run build` → `dist/` → FTP / Vercel / Netlify
- **CI/CD**: Azure DevOps Pipelines (build, test, deploy)

## Estado del Proyecto

- �� Backend API completa (Auth, CRUDs, JWT, EF Core)
- �� Frontend React funcional (routing, services, context, UI)
- �� Base de datos PostgreSQL con migraciones
- ��� Integración completa Frontend���Backend en progreso
- ��� Tests unitarios/integración pendientes

## Autor

**Alex Joan Roblero Quirós**  
Universidad Hispanoamericana — 8º Cuatrimestre  
Proyecto académico / Portafolio profesional