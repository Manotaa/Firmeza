#!/usr/bin/env bash
# ==============================================================================
# Script de Ingeniería Inversa Database-First: Scaffold-DbContext
# Genera modelos y ApplicationDbContext a partir del esquema PostgreSQL
# ==============================================================================
set -e

CONNECTION_STRING="Host=localhost;Port=5432;Database=firmeza_db;Username=postgres;Password=postgres"

echo "=== Ejecutando Scaffold-DbContext (Database-First) con EF Core y Npgsql ==="
echo "Cadena de conexión: $CONNECTION_STRING"

dotnet ef dbcontext scaffold "$CONNECTION_STRING" Npgsql.EntityFrameworkCore.PostgreSQL \
  --project ../src/Firmeza.Infrastructure/Firmeza.Infrastructure.csproj \
  --startup-project ../src/Firmeza.Api/Firmeza.Api.csproj \
  --output-dir Persistence/Scaffolded \
  --context ApplicationDbContext \
  --force \
  --no-build

echo "=== Proceso Database-First completado exitosamente ==="
