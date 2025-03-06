# Usa la imagen base de .NET SDK para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copia el archivo .csproj y restaura las dependencias
COPY TodoApi.csproj .
RUN dotnet nuget locals all --clear
RUN dotnet restore

# Copia todo el código fuente y construye la aplicación
COPY . .
RUN dotnet publish -c Release -o /app --no-restore

# Usa la imagen base de .NET Runtime para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copia los archivos publicados desde la etapa de compilación
COPY --from=build /app .

# Expone el puerto que usará la aplicación
EXPOSE 80

# Define la variable de entorno para el puerto dinámico de Render
ENV ASPNETCORE_URLS=http://*:$PORT

# Comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "TodoApi.dll"]