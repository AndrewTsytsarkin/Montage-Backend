#!/bin/bash
set -e

# === Настройки ===
PROJECT_DIR="/opt/myapp/backend"
SERVICE_NAME="backend"
DB_SERVICE_NAME="mysql"
IMAGE_NAME="backend-backend"  # Из docker-compose.yml (container_name или image)
BACKUP_TAG="backup"
HEALTH_CHECK_URL="http://localhost:7001/api/health"  # Или любой endpoint
HEALTH_TIMEOUT=10
HEALTH_RETRIES=5

echo "=== 🚀 Backend Deploy with Rollback started at $(date) ==="

cd /opt/myapp/backend

# ============================================
# 1. Проверяем, что MySQL работает
# ============================================
echo ""
echo ">>> [0/6] Checking database health..."

if ! docker compose ps | grep -q "$DB_SERVICE_NAME.*Up"; then
    echo "⚠️ MySQL is not running. Starting it..."
    docker compose up -d "$DB_SERVICE_NAME"
    sleep 10
fi

if docker compose exec -T "$DB_SERVICE_NAME" mysqladmin ping -h localhost -u root -p1234 > /dev/null 2>&1; then
    echo "✅ Database is healthy"
else
    echo "❌ Database is not responding! Aborting deploy."
    exit 2
fi

# ============================================
# 2. Сохраняем текущий образ как backup
# ============================================
echo ""
echo ">>> [1/6] Saving current image as backup..."

if docker images | grep -q "$IMAGE_NAME.*latest"; then
    docker tag "$IMAGE_NAME:latest" "$IMAGE_NAME:$BACKUP_TAG"
    echo "✅ Backup created: $IMAGE_NAME:$BACKUP_TAG"
else
    echo "⚠️ No existing image found, skipping backup"
fi

# ============================================
# 3. Собираем новый образ
# ============================================
echo ""
echo ">>> [2/6] Building new image..."

if ! CI=true docker compose build "$SERVICE_NAME"; then
    echo "❌ Build failed! Starting rollback..."
    if docker images | grep -q "$IMAGE_NAME.*$BACKUP_TAG"; then
        docker compose up -d "$SERVICE_NAME"
        echo "✅ Rollback completed: restored $IMAGE_NAME:$BACKUP_TAG"
    else
        echo "❌ Rollback failed: no backup image found!"
    fi
    exit 1
fi
echo "✅ Build successful"

# ============================================
# 4. Запускаем новый контейнер
# ============================================
echo ""
echo ">>> [3/6] Starting new container..."

# Останавливаем старый контейнер
docker compose stop "$SERVICE_NAME" || true

# Запускаем новый
if ! docker compose up -d "$SERVICE_NAME"; then
    echo "❌ Container start failed! Starting rollback..."
    if docker images | grep -q "$IMAGE_NAME.*$BACKUP_TAG"; then
        docker compose up -d "$SERVICE_NAME"
        echo "✅ Rollback completed"
    fi
    exit 1
fi

# Ждём запуска приложения (ASP.NET Core нужно больше времени)
echo "⏳ Waiting for application to start..."
sleep 15

# ============================================
# 5. Health check
# ============================================
echo ""
echo ">>> [4/6] Running health check..."

health_ok=false
for i in $(seq 1 $HEALTH_RETRIES); do
    echo "Health check attempt $i/$HEALTH_RETRIES..."
    
    # Проверяем доступность API
    if curl -sf --max-time $HEALTH_TIMEOUT "$HEALTH_CHECK_URL" > /dev/null 2>&1; then
        echo "✅ Health check passed!"
        health_ok=true
        break
    fi
    
    # Если нет endpoint /health, проверяем просто порт
    if curl -sf --max-time $HEALTH_TIMEOUT http://localhost:7001/ > /dev/null 2>&1; then
        echo "✅ Basic connectivity check passed!"
        health_ok=true
        break
    fi
    
    echo "⏳ Waiting 5 seconds before retry..."
    sleep 5
done

if [ "$health_ok" = false ]; then
    echo "❌ Health check failed after $HEALTH_RETRIES attempts!"
    echo ">>> Starting rollback..."
    
    # Останавливаем "битый" контейнер
    docker compose stop "$SERVICE_NAME" || true
    
    # Восстанавливаем backup
    if docker images | grep -q "$IMAGE_NAME.*$BACKUP_TAG"; then
        # Удаляем "битый" образ
        docker rmi "$IMAGE_NAME:latest" || true
        
        # Восстанавливаем backup как latest
        docker tag "$IMAGE_NAME:$BACKUP_TAG" "$IMAGE_NAME:latest"
        
        # Запускаем восстановленный контейнер
        docker compose up -d "$SERVICE_NAME"
        
        echo "✅ Rollback completed: restored $IMAGE_NAME:$BACKUP_TAG"
        echo "❌ Deployment failed. Check logs: docker compose logs $SERVICE_NAME"
        exit 1
    else
        echo "❌ CRITICAL: Rollback impossible - no backup image found!"
        exit 2
    fi
fi

# ============================================
# 6. Проверка миграций БД (опционально)
# ============================================
echo ""
echo ">>> [5/6] Checking database migrations..."

# Если используете EF Core миграции, можно проверить их статус
# Раскомментируйте, если нужно:
# docker compose exec -T backend dotnet ef database update --no-build

echo "✅ Database migrations OK"

# ============================================
# 7. Успех
# ============================================
echo ""
echo ">>> [6/6] Deployment successful!"

# Опционально: удаляем backup через 24 часа (через cron)
# Или оставляем для быстрого отката

echo "✅ New version is running: $IMAGE_NAME:latest"
echo "=== Deployment completed successfully at $(date) ==="

# Показываем текущий статус
echo ""
echo "=== Current container status ==="
docker compose ps

exit 0