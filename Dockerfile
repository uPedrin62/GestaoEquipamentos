FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copia tudo
COPY . .

# Restaura usando o csproj diretamente
RUN dotnet restore GestaoEquipamentos.csproj

# Publica
RUN dotnet publish GestaoEquipamentos.csproj -c Release -o out

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 10000
ENTRYPOINT ["dotnet", "GestaoEquipamentos.csproj"]
