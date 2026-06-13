FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Banking.Domain/Banking.Domain.csproj src/Banking.Domain/
COPY src/Banking.Application/Banking.Application.csproj src/Banking.Application/
COPY src/Banking.Infrastructure/Banking.Infrastructure.csproj src/Banking.Infrastructure/
COPY src/Banking.API/Banking.API.csproj src/Banking.API/

RUN dotnet restore src/Banking.API/Banking.API.csproj

COPY src/ src/

RUN dotnet publish src/Banking.API/Banking.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

HEALTHCHECK --interval=10s --timeout=5s --retries=10 --start-period=30s \
  CMD curl -f http://localhost:8080/swagger/index.html || exit 1

ENTRYPOINT ["dotnet", "Banking.API.dll"]
