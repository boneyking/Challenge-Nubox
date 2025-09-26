#!/bin/bash
echo "=== Configurando Integración Nubox con Docker ==="

# Crear directorios necesarios
mkdir -p logs temp uploads monitoring/prometheus monitoring/grafana/provisioning nginx/ssl

# Crear archivos de configuración si no existen
if [ ! -f "rabbitmq.conf" ]; then
    echo "Creando configuración de RabbitMQ..."
    cat > rabbitmq.conf << EOF
management.tcp.port = 15672
management.tcp.ip = 0.0.0.0
loopback_users.guest = false
listeners.tcp.default = 5672
default_pass = admin123
default_user = admin
EOF
fi

# Build y start
echo "Construyendo y levantando servicios..."
docker-compose down
docker-compose build --no-cache
docker-compose up -d

# Esperar a que los servicios estén listos
echo "Esperando a que los servicios estén listos..."
sleep 30

# Verificar health checks
echo "Verificando servicios..."
docker-compose ps

echo "=== Setup completado ==="
echo "Servicios disponibles:"
echo "- API: http://localhost:8080"
echo "- Swagger: http://localhost:8080/swagger"
echo "- Health: http://localhost:8080/health"
echo "- RabbitMQ: http://localhost:15672 (admin/admin123)"
echo "- Grafana: http://localhost:3000 (admin/admin123)"
echo "- Prometheus: http://localhost:9090"
