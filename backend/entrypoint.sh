#!/bin/bash
set -e

# Ожидание доступности PostgreSQL
until PGPASSWORD=$DB_PASSWORD psql -h db -U "$DB_USER" -d "$DB_NAME" -c '\q'; do
  >&2 echo "PostgreSQL is unavailable - sleeping"
  sleep 1
done

# Применение миграций
>&2 echo "Applying database migrations..."
PGPASSWORD="$DB_PASSWORD" psql -h db -U "$DB_USER" -d "$DB_NAME" -f migrate.sql

# Запуск приложения
>&2 echo "Starting application..."
exec dotnet SmartEstate.API.dll