FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder
WORKDIR /src

# 1. Копируем решение и файлы проектов (для кэширования слоя restore)
COPY ["Back2.sln", "./"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["WebApplication1/MontageAPI.csproj", "WebApplication1/"]

# 2. Восстанавливаем пакеты ДЛЯ ВСЕГО РЕШЕНИЯ
# Это корректно разрешит все ProjectReference, даже с относительными путями
RUN dotnet restore "Back2.sln"

# 3. Копируем весь исходный код
COPY ["Data/", "Data/"]
COPY ["WebApplication1/", "WebApplication1/"]

# 4. Собираем и публикуем, указывая путь к проекту API
RUN dotnet build "WebApplication1/MontageAPI.csproj" -c Release -o /app/build

FROM builder AS publish
RUN dotnet publish "WebApplication1/MontageAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV TZ=Europe/Moscow

RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

ENTRYPOINT ["dotnet", "MontageAPI.dll"]