# Sistema de Integración Nubox

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoft-sql-server)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3.x-FF6600?logo=rabbitmq)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker)

Sistema de integración para sincronizar datos de asistencia entre el Sistema de Control de Asistencia y el Sistema de Cálculo de Liquidaciones, diseñado para ser **seguro, escalable, observable, trazable y resiliente**.

## 📋 Tabla de Contenidos

- [Contexto del Problema](#-contexto-del-problema)
- [Arquitectura de la Solución](#️-arquitectura-de-la-solución)
- [Tecnologías Utilizadas](#-tecnologías-utilizadas)
- [Instalación](#-instalación)
- [Configuración](#️-configuración)
- [Uso de la API](#-uso-de-la-api)
- [Monitoreo](#-monitoreo)
- [Estructura del Proyecto](#-estructura-del-proyecto)

## 🎯 Contexto del Problema

### Desafío de Negocio

El sistema resuelve la integración entre dos sistemas críticos:

- **Sistema de Cálculo de Liquidaciones**: Procesa nóminas (semanal, quincenal, mensual) para **100,000 empresas**
- **Sistema de Control de Asistencia**: Proveedor externo que registra marcajes con dispositivos físicos/tecnológicos

### Requerimientos Clave

✅ Intercambio seguro de información entre sistemas  
✅ Procesamiento de datos de asistencia (horas, extras, licencias)  
✅ Soporte para APIs y archivos Excel  
✅ Arquitectura escalable, observable, trazable y resiliente  

## 🏗️ Arquitectura de la Solución

```
┌─────────────────────────────────────────────┐
│    SISTEMA DE CONTROL DE ASISTENCIA        │
│     (APIs + Excel con datos de marcaje)    │
└────────────────────┬────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────┐
│         API DE INTEGRACIÓN NUBOX            │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  │
│  │   Auth   │  │  Nómina  │  │Validación│  │
│  │  (JWT)   │  │Endpoints │  │de Datos  │  │
│  └──────────┘  └──────────┘  └──────────┘  │
└────────────────────┬────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────┐
│       RABBITMQ (MESSAGE BROKER)             │
│    Exchange: "Nomina" | Type: Direct        │
└────────────────────┬────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────┐
│       PROCESAMIENTO ASÍNCRONO               │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  │
│  │Subscriber│  │Bulk Insert│ │ Bitácora │  │
│  └──────────┘  └──────────┘  └──────────┘  │
└────────────────────┬────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────┐
│          SQL SERVER (Persistencia)          │
│  Auth DB  |  IntegracionNubox DB            │
└─────────────────────────────────────────────┘
```

### Características Principales

#### 🔒 Seguridad
- Autenticación JWT con tokens de 24 horas
- Validación de compañías antes de procesar
- Usuario Docker no-root
- Contraseñas hasheadas

#### 📈 Escalabilidad
- Procesamiento asíncrono con RabbitMQ
- 3 estrategias de bulk insert según volumen:
  - **SqlBulkCopy**: >10,000 registros
  - **EF Core Optimizado**: 1,000-10,000 registros
  - **EF Core Normal**: <1,000 registros
- API stateless para escalado horizontal

#### 👁️ Observabilidad
- Logging estructurado con Serilog
- Métricas con Prometheus
- Dashboards en Grafana
- Health checks

#### 🔍 Trazabilidad
- BitacoraSincronizacion por cada operación
- Timestamps en eventos
- Relación con Compañía
- Detalles de trabajador procesado

#### 💪 Resiliencia
- Reintentos automáticos en RabbitMQ
- Manual ACK para garantizar procesamiento
- Transacciones en bulk operations
- Network recovery habilitado

## 💻 Tecnologías Utilizadas

### Stack Principal
- **.NET 8.0** - Framework base
- **ASP.NET Core 8.0** - Web API
- **Entity Framework Core 8.0** - ORM
- **SQL Server 2022** - Base de datos
- **RabbitMQ 3** - Message broker
- **Redis 7** - Cache distribuido

### Librerías Clave
- **MediatR** - CQRS pattern
- **EPPlus** - Procesamiento de Excel
- **Serilog** - Logging estructurado
- **JWT Bearer** - Autenticación

### Monitoreo
- **Prometheus** - Métricas
- **Grafana** - Visualización
- **Docker Compose** - Orquestación

## 🚀 Instalación

### Prerrequisitos

- Docker y Docker Compose
- .NET 8.0 SDK
- Git

### Paso 1: Clonar el Repositorio

```bash
git clone <repository-url>
cd Integracion.Nubox
```

### Paso 2: Configurar Variables de Entorno

Editar `Integracion.Nubox.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "AuthConnection": "Server=127.0.0.1,1433;Database=Auth;User ID=authlogin;Password=Fatel4707!@;TrustServerCertificate=True",
    "IntegracionNuboxConnection": "Server=127.0.0.1,1433;Database=IntegracionNubox;User ID=integracionNuboxlogin;Password=Fatel4707!@;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "Secret": "your-very-long-secret-key-at-least-32-chars!",
    "ValidAudience": "your-audience",
    "ValidIssuer": "your-issuer"
  },
  "IntegracionSettings": {
    "RabbitConfiguration": {
      "HostRabbitMQ": "localhost",
      "PortRabbitMQ": 5672,
      "Username": "admin",
      "Password": "admin123",
      "Exchange": "Nomina"
    }
  }
}
```

### Paso 3: Levantar Infraestructura

```bash
cd Integracion.Nubox.Api
docker-compose up -d
```

Servicios levantados:
- SQL Server → `localhost:1433`
- RabbitMQ → `localhost:5672` (Management: `localhost:15672`)
- Redis → `localhost:6379`
- Prometheus → `localhost:9090`
- Grafana → `localhost:3000`

### Paso 4: Ejecutar Migraciones

```bash
# Base de datos de Autenticación
dotnet ef database update --context AuthContext

# Base de datos de Integración
dotnet ef database update --context IntegracionNuboxContext
```

### Paso 5: Ejecutar la API

```bash
dotnet run --project Integracion.Nubox.Api
```

Acceder a Swagger: `https://localhost:7224/swagger`

## ⚙️ Configuración

### Base de Datos

#### Auth Database
```sql
Table: Users
- Id: uniqueidentifier (PK)
- Username: nvarchar
- PasswordHash: nvarchar
- IsActive: bit
```

#### IntegracionNubox Database
```sql
Table: Companias
- Id: uniqueidentifier (PK)
- Nombre: nvarchar

Table: BitacoraSincronizacion
- Id: uniqueidentifier (PK)
- CompaniaId: uniqueidentifier (FK)
- FechaSincronizacion: datetime2
- Detalles: nvarchar
```

### RabbitMQ

El sistema configura automáticamente:
- **Exchange**: `Nomina` (Direct, Durable)
- **Queue**: `SincronizarNominaEvent` (Durable)
- **Routing Key**: `SincronizarNominaEvent`

Acceso a Management UI:
- URL: `http://localhost:15672`
- Usuario: `admin`
- Password: `admin123`

## 📡 Uso de la API

### Autenticación

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "hashed_password"
}
```

**Respuesta:**
```json
{
  "status": true,
  "message": "OK",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "expires": "2025-09-26T12:00:00Z"
  }
}
```

### Sincronizar Nómina

```http
POST /api/nomina/sincronizar
Authorization: Bearer {token}
Content-Type: application/json

{
  "idCompania": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "origen": 2,
  "trabajadores": [
    {
      "idTrabajador": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
      "nombresTrabajador": "Juan",
      "apellidosTrabajador": "Pérez",
      "dniTrabajador": "12345678"
    }
  ]
}
```

**Respuesta:**
```json
{
  "status": true,
  "message": "Nómina sincronizada exitosamente",
  "data": {
    "trabajadoresProcesados": 1
  }
}
```

### Upload Excel

```http
POST /api/auth/upload-excel
Authorization: Bearer {token}
Content-Type: multipart/form-data

file: [archivo.xlsx]
```

### Health Check

```http
GET /auth/health
Authorization: Bearer {token}
```

## 📊 Monitoreo

### Prometheus
- **URL**: `http://localhost:9090`
- Métricas del API, RabbitMQ, SQL Server

### Grafana
- **URL**: `http://localhost:3000`
- **Usuario**: admin
- **Password**: admin123
- Dashboards preconfigurados para:
  - Throughput de mensajes
  - Latencia de procesamiento
  - Uso de recursos

### Logs
- **Framework**: Serilog
- **Ubicación**: `./logs/`
- **Formato**: JSON estructurado

## 📁 Estructura del Proyecto

```
Integracion.Nubox/
├── Integracion.Nubox.Api/
│   ├── Common/
│   │   ├── Entities/          # Entidades base y enums
│   │   ├── Persistence/       # Repositorios base
│   │   └── Services/          # Servicios comunes (RabbitMQ)
│   ├── Features/
│   │   ├── Auth/              # Autenticación y autorización
│   │   │   ├── Endpoints/
│   │   │   ├── Handlers/
│   │   │   └── Requests/
│   │   └── Nomina/            # Funcionalidad de nómina
│   │       ├── Endpoints/
│   │       ├── Events/
│   │       ├── Handlers/
│   │       ├── Publishers/
│   │       ├── Subscribers/
│   │       └── Requests/
│   ├── Infrastructure/
│   │   ├── Persistence/
│   │   │   └── Contexts/
│   │   │       ├── Auth/      # Contexto de autenticación
│   │   │       └── IntegracionNubox/  # Contexto principal
│   │   └── Services/          # Servicios de infraestructura
│   ├── Extensions/            # Extensiones de configuración
│   ├── docker-compose.yml     # Orquestación de servicios
│   ├── Dockerfile             # Imagen de la API
│   └── appsettings.json       # Configuración
└── README.md
```

## 🔄 Flujo de Sincronización

1. **Cliente autentica** → Obtiene JWT token
2. **Cliente envía nómina** → API valida y publica a RabbitMQ
3. **RabbitMQ encola mensaje** → Garantiza entrega
4. **Subscriber procesa** → Selecciona estrategia de bulk insert
5. **Datos persisten** → SQL Server + BitacoraSincronizacion
6. **Sistema confirma** → ACK a RabbitMQ

## 🛠️ Comandos Útiles

### Docker
```bash
# Levantar servicios
docker-compose up -d

# Ver logs
docker-compose logs -f

# Detener servicios
docker-compose down

# Limpiar volúmenes
docker-compose down -v
```

### Entity Framework
```bash
# Crear migración
dotnet ef migrations add MigrationName --context AuthContext

# Aplicar migraciones
dotnet ef database update --context IntegracionNuboxContext

# Eliminar última migración
dotnet ef migrations remove --context AuthContext
```

### RabbitMQ
```bash
# Ver estado de queues
curl -u admin:admin123 http://localhost:15672/api/queues

# Purgar queue
curl -u admin:admin123 -X DELETE http://localhost:15672/api/queues/%2F/SincronizarNominaEvent/contents
```

## 🧪 Testing

### Usando Swagger
1. Navegar a `https://localhost:7224/swagger`
2. Ejecutar `/api/auth/login` para obtener token
3. Hacer clic en "Authorize" y pegar el token
4. Probar endpoints protegidos

### Usando cURL

```bash
# Login
curl -X POST https://localhost:7224/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"hashed_password"}'

# Sincronizar Nómina
curl -X POST https://localhost:7224/api/nomina/sincronizar \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "idCompania":"3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "origen":2,
    "trabajadores":[{
      "idTrabajador":"3fa85f64-5717-4562-b3fc-2c963f66afa7",
      "nombresTrabajador":"Juan",
      "apellidosTrabajador":"Pérez",
      "dniTrabajador":"12345678"
    }]
  }'
```

## 🤝 Contribución

1. Fork el repositorio
2. Crear rama feature (`git checkout -b feature/AmazingFeature`)
3. Commit cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abrir Pull Request

## 📝 Patrones de Diseño

- **CQRS** con MediatR
- **Repository Pattern**
- **Unit of Work**
- **Publisher/Subscriber**
- **Strategy Pattern** (Bulk Insert)
- **Dependency Injection**

## 📄 Licencia

Este proyecto es privado y confidencial.

## 👥 Contacto

Para consultas sobre el proyecto, contactar al equipo de desarrollo.

---

**Desarrollado con ❤️ para Nubox**