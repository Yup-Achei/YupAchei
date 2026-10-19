# Multi-stage build para YupAchei.API (.NET 10)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copia arquivos de configuração e projetos para cache de camadas
COPY nuget.config ./
COPY src/YupAchei.Domain/YupAchei.Domain.csproj src/YupAchei.Domain/
COPY src/YupAchei.Application/YupAchei.Application.csproj src/YupAchei.Application/
COPY src/YupAchei.Infrastructure/YupAchei.Infrastructure.csproj src/YupAchei.Infrastructure/
COPY src/YupAchei.API/YupAchei.API.csproj src/YupAchei.API/

RUN dotnet restore src/YupAchei.API/YupAchei.API.csproj

# Copia código-fonte e compila
COPY src/ src/
RUN dotnet publish src/YupAchei.API/YupAchei.API.csproj -c Release -o /app/publish --no-restore

# Imagem de execução ASP.NET Core (.NET 10)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "YupAchei.API.dll"]

