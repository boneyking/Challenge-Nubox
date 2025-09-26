# Comandos cURL - Sistema Integración Nubox

## 🔐 1. Login (Obtener Token JWT)

### Windows (CMD)
```cmd
curl -X POST https://localhost:7224/api/auth/login ^
  -H "Content-Type: application/json" ^
  -d "{\"username\":\"admin\",\"password\":\"hashed_password\"}" ^
  -k
```

### Windows (PowerShell)
```powershell
curl.exe -X POST https://localhost:7224/api/auth/login `
  -H "Content-Type: application/json" `
  -d '{\"username\":\"admin\",\"password\":\"hashed_password\"}' `
  -k
```

### Linux/Mac
```bash
curl -X POST https://localhost:7224/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"hashed_password"}' \
  -k
```

**Respuesta esperada:**
```json
{
  "status": true,
  "message": "OK",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expires": "2025-09-26T12:00:00Z"
  }
}
```

---

## 📤 2. Upload Excel

### Windows (CMD)
```cmd
curl -X POST https://localhost:7224/api/auth/upload-excel ^
  -H "Authorization: Bearer TU_TOKEN_AQUI" ^
  -F "file=@Trabajadores_100K.xlsx" ^
  -k
```

### Windows (PowerShell)
```powershell
curl.exe -X POST https://localhost:7224/api/auth/upload-excel `
  -H "Authorization: Bearer TU_TOKEN_AQUI" `
  -F "file=@Trabajadores_100K.xlsx" `
  -k
```

### Linux/Mac
```bash
curl -X POST https://localhost:7224/api/auth/upload-excel \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  -F "file=@Trabajadores_100K.xlsx" \
  -k
```

**Nota:** Reemplaza `TU_TOKEN_AQUI` con el token obtenido en el login.

**Respuesta esperada:**
```json
{
  "rows": [
    ["IdTrabajador", "NombresTrabajador", "ApellidosTrabajador", "DniTrabajador", "..."],
    ["guid-1", "Juan", "García", "12345678", "..."],
    ["guid-2", "María", "Rodríguez", "87654321", "..."]
  ]
}
```

---

## 📋 3. Sincronizar Nómina

### Windows (CMD)
```cmd
curl -X POST https://localhost:7224/api/nomina/sincronizar ^
  -H "Authorization: Bearer TU_TOKEN_AQUI" ^
  -H "Content-Type: application/json" ^
  -d "{\"idCompania\":\"3fa85f64-5717-4562-b3fc-2c963f66afa6\",\"origen\":2,\"trabajadores\":[{\"idTrabajador\":\"3fa85f64-5717-4562-b3fc-2c963f66afa7\",\"nombresTrabajador\":\"Juan\",\"apellidosTrabajador\":\"Pérez\",\"dniTrabajador\":\"12345678\"}]}" ^
  -k
```

### Windows (PowerShell) - Usando archivo JSON
```powershell
# Crear archivo nomina.json primero con el contenido
curl.exe -X POST https://localhost:7224/api/nomina/sincronizar `
  -H "Authorization: Bearer TU_TOKEN_AQUI" `
  -H "Content-Type: application/json" `
  -d '@nomina.json' `
  -k
```

### Linux/Mac
```bash
curl -X POST https://localhost:7224/api/nomina/sincronizar \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  -H "Content-Type: application/json" \
  -d '{
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
  }' \
  -k
```

**Respuesta esperada:**
```json
{
  "status": true,
  "message": "Nómina sincronizada exitosamente",
  "data": {
    "trabajadoresProcesados": 1
  }
}
```

---

## ❤️ 4. Health Check

### Todas las plataformas
```bash
curl -X GET https://localhost:7224/auth/health \
  -H "Authorization: Bearer TU_TOKEN_AQUI" \
  -k
```

**Respuesta esperada:**
```json
{
  "status": "Healthy"
}
```

---

## 🔄 Flujo Completo (Ejemplo)

### 1. Obtener Token
```bash
TOKEN=$(curl -X POST https://localhost:7224/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"hashed_password"}' \
  -k -s | jq -r '.data.token')

echo "Token obtenido: $TOKEN"
```

### 2. Upload Excel con Token
```bash
curl -X POST https://localhost:7224/api/auth/upload-excel \
  -H "Authorization: Bearer $TOKEN" \
  -F "file=@Trabajadores_100K.xlsx" \
  -k
```

### 3. Sincronizar Nómina con Token
```bash
curl -X POST https://localhost:7224/api/nomina/sincronizar \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "idCompania": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "origen": 2,
    "trabajadores": [
      {
        "idTrabajador": "guid-here",
        "nombresTrabajador": "Juan",
        "apellidosTrabajador": "Pérez",
        "dniTrabajador": "12345678"
      }
    ]
  }' \
  -k
```

---

## 📝 Notas Importantes

### Sobre `-k` o `--insecure`
- Se usa para ignorar validación SSL en desarrollo con certificados autofirmados
- **NUNCA usar en producción**

### Sobre el Token
- El token expira en 24 horas
- Guarda el token en una variable para reutilizarlo
- Si expira, vuelve a hacer login

### Sobre las rutas de archivo
- `@Trabajadores_100K.xlsx` - archivo en la carpeta actual
- `@C:\ruta\completa\archivo.xlsx` - ruta absoluta Windows
- `@/home/user/archivo.xlsx` - ruta absoluta Linux/Mac

### Obtener un ID de Compañía válido
```bash
# Si tienes SQL Server accesible
sqlcmd -S localhost -U sa -P YourPassword -Q "SELECT TOP 1 Id FROM IntegracionNubox.dbo.Companias"
```

---

## 🛠️ Scripts de Ayuda

### Script PowerShell Completo
```powershell
# 1. Login
$response = curl.exe -X POST https://localhost:7224/api/auth/login `
  -H "Content-Type: application/json" `
  -d '{\"username\":\"admin\",\"password\":\"hashed_password\"}' `
  -k | ConvertFrom-Json

$token = $response.data.token
Write-Host "Token: $token"

# 2. Upload Excel
curl.exe -X POST https://localhost:7224/api/auth/upload-excel `
  -H "Authorization: Bearer $token" `
  -F "file=@Trabajadores_100K.xlsx" `
  -k
```

### Script Bash Completo
```bash
#!/bin/bash

# 1. Login y obtener token
TOKEN=$(curl -X POST https://localhost:7224/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"hashed_password"}' \
  -k -s | jq -r '.data.token')

echo "Token obtenido: ${TOKEN:0:50}..."

# 2. Upload Excel
echo "Subiendo archivo Excel..."
curl -X POST https://localhost:7224/api/auth/upload-excel \
  -H "Authorization: Bearer $TOKEN" \
  -F "file=@Trabajadores_100K.xlsx" \
  -k

echo "Proceso completado!"
```

---

## 🚨 Troubleshooting

### Error: "curl: command not found" en Windows
- Usar `curl.exe` en lugar de `curl` en PowerShell
- O instalar curl desde https://curl.se/windows/

### Error: "SSL certificate problem"
- Agregar `-k` o `--insecure` al comando
- Solo para desarrollo, no producción

### Error: "401 Unauthorized"
- Verificar que el token sea correcto
- Verificar que no haya expirado (24 horas)
- Volver a hacer login

### Error: "File not found"
- Verificar ruta del archivo Excel
- Usar ruta absoluta si es necesario
- Verificar que el archivo existe con `ls` o `dir`