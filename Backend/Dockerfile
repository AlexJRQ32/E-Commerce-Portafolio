# Etapa 1: Compilación (Build)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar el archivo de proyecto .csproj y restaurar dependencias
COPY ["Backend/Backend.csproj", "Backend/"]
RUN dotnet restore "Backend/Backend.csproj"

# Copiar todo el resto del código fuente del backend
COPY . .
WORKDIR "/src/Backend"

# Compilar en modo Release hacia el directorio /app/publish
RUN dotnet publish "Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Entorno de ejecución (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Exponer el puerto por el que escuchará el contenedor
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Comando para arrancar la aplicación
ENTRYPOINT ["dotnet", "Backend.dll"]
