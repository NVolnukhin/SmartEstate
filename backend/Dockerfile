# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копирование манифеста инструментов
COPY ./backend/WebApplication/.config/dotnet-tools.json ./.config/dotnet-tools.json

# Восстановление инструментов (включая dotnet-ef)
RUN dotnet tool restore

# Копирование файлов проектов
COPY ./backend/WebApplication/WebApplication.sln .
COPY ./backend/WebApplication/SmartEstate.API/*.csproj ./SmartEstate.API/
COPY ./backend/WebApplication/SmartEstate.Application/*.csproj ./SmartEstate.Application/
COPY ./backend/WebApplication/SmartEstate.Contracts/*.csproj ./SmartEstate.Contracts/
COPY ./backend/WebApplication/SmartEstate.DataAccess/*.csproj ./SmartEstate.DataAccess/
COPY ./backend/WebApplication/SmartEstate.DatabaseContext/*.csproj ./SmartEstate.DatabaseContext/
COPY ./backend/WebApplication/SmartEstate.DatabaseModel/*.csproj ./SmartEstate.DatabaseModel/
COPY ./backend/WebApplication/SmartEstate.Email/*.csproj ./SmartEstate.Email/
COPY ./backend/WebApplication/SmartEstate.Infrastructure/*.csproj ./SmartEstate.Infrastructure/
COPY ./backend/WebApplication/SmartEstate.Routing/*.csproj ./SmartEstate.Routing/

# Восстановление зависимостей
RUN dotnet restore "WebApplication.sln"

# Копирование остального кода
COPY ./backend/WebApplication/. .

# Генерация SQL-скрипта миграций
RUN dotnet tool run dotnet-ef migrations script --idempotent -o migrate.sql \
    --project SmartEstate.DatabaseContext/SmartEstate.DatabaseContext.csproj \
    --startup-project SmartEstate.API/SmartEstate.API.csproj

# Публикация проекта
RUN dotnet publish "SmartEstate.API/SmartEstate.API.csproj" -c Release -o /app/publish

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Копирование собранного кода и миграционного скрипта
COPY --from=build /app/publish .
COPY --from=build /src/migrate.sql .

# Добавление виртуального окружения в PATH
ENV PATH="/venv/bin:$PATH"

# Установка клиента PostgreSQL
RUN apt-get update && \
    apt-get install -y postgresql-client && \
    rm -rf /var/lib/apt/lists/*

# Копирование скрипта запуска
COPY ./backend/entrypoint.sh .
RUN chmod +x entrypoint.sh

EXPOSE 5000
ENTRYPOINT ["./entrypoint.sh"]
