# Build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copia tudo
COPY . ./

# Vai para a pasta do projeto
WORKDIR /app/GestaoEquipamentos

# Restaura
RUN dotnet restore

# Publica
RUN dotnet publish -c Release -o out

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/GestaoEquipamentos/out ./

EXPOSE 10000
ENTRYPOINT ["dotnet", "GestaoEquipamentos.dll"]
