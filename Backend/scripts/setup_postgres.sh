#!/usr/bin/env bash
set -e

echo "=== Configuración de PostgreSQL para Proyecto Firmeza ==="

# 1. Asegurar que el servicio PostgreSQL esté activo
sudo systemctl start postgresql

# 2. Configurar la contraseña del superusuario postgres a 'postgres'
sudo -u postgres psql -c "ALTER USER postgres WITH PASSWORD 'postgres';"

# 3. Crear base de datos firmeza_db si no existe
sudo -u postgres psql -tc "SELECT 1 FROM pg_database WHERE datname = 'firmeza_db'" | grep -q 1 || \
sudo -u postgres psql -c "CREATE DATABASE firmeza_db OWNER postgres;"

# 4. Asignar todos los privilegios
sudo -u postgres psql -c "GRANT ALL PRIVILEGES ON DATABASE firmeza_db TO postgres;"

echo "=== Base de datos firmeza_db y usuario postgres configurados con éxito ==="
echo "Probando conexión..."
PGPASSWORD=postgres psql -h localhost -U postgres -d firmeza_db -c "SELECT 'Conexión exitosa a firmeza_db' AS estado;"
