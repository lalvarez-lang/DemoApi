# Utilizar imagen Base .NET 9.0 SDK para permitir migraciones EF
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Instalar EF Core Tools globalmente en la imagen base
RUN dotnet tool install --global dotnet-ef
ENV PATH="/root/.dotnet/tools:$PATH"

FROM base AS build
WORKDIR /src
COPY ["DemoApi.csproj", "./"]
RUN dotnet restore "./DemoApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "DemoApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DemoApi.csproj" -c Release -o /app/publish

# Imagen para producción (solo ejecuta la app)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoApi.dll"]

# Imagen para migraciones (incluye el SDK, EF Tools y el código fuente)
FROM base AS migration
WORKDIR /app
COPY . .
ENTRYPOINT ["/bin/bash"]