# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copiar archivos del proyecto
COPY . .

# Restaurar dependencias
RUN dotnet restore

# Compilar y publicar
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

# Puerto dinámico de Render
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}

EXPOSE 8080

ENTRYPOINT ["dotnet", "TuProyecto.dll"]