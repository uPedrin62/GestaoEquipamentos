# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copia csproj e restaura dependências
COPY *.csproj ./
RUN dotnet restore

# Copia todo o código e publica
COPY . ./
RUN dotnet publish -c Release -o out

# Imagem final
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out ./

# Porta usada pelo Render
EXPOSE 10000

# Rodar aplicação
ENTRYPOINT ["dotnet", "GestaoEquipamentos.dll"]
