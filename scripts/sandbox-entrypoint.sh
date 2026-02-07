#!/bin/bash
set -e

echo "Running database migrations..."
cd /home/app/src/App
dotnet ef database update --no-build

echo "Migrations completed successfully!"

echo "Starting Contest App API..."
exec "$@" 
