FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Crear usuario no-root para seguridad
RUN addgroup --system --gid 1001 dotnet
RUN adduser --system --uid 1001 --ingroup dotnet dotnet

# Instalar curl para health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["Integracion.Nubox.Api.csproj", "Integracion.Nubox.Api/"]
RUN dotnet restore "Integracion.Nubox.Api/Integracion.Nubox.Api.csproj"

# Copiar código fuente
COPY . Integracion.Nubox.Api/
WORKDIR "/src/Integracion.Nubox.Api"

# Build
RUN dotnet build "Integracion.Nubox.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Integracion.Nubox.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Crear directorios necesarios para la aplicación
RUN mkdir -p /app/logs /app/temp /app/uploads && chown -R dotnet:dotnet /app

# Copiar archivos publicados
COPY --from=publish /app/publish .

# Variables de entorno
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_HTTP_PORTS=80

# Cambiar a usuario no-root
USER dotnet

# Health check mejorado
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD curl -f http://localhost:80/health/ready || exit 1

ENTRYPOINT ["dotnet", "Integracion.Nubox.Api.dll"]