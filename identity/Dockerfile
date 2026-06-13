FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Identity.Domain/Identity.Domain.csproj src/Identity.Domain/
COPY src/Identity.Application/Identity.Application.csproj src/Identity.Application/
COPY src/Identity.Infrastructure/Identity.Infrastructure.csproj src/Identity.Infrastructure/
COPY src/Identity.API/Identity.API.csproj src/Identity.API/

RUN dotnet restore src/Identity.API/Identity.API.csproj

COPY src/ src/

RUN dotnet publish src/Identity.API/Identity.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

HEALTHCHECK --interval=10s --timeout=5s --retries=10 --start-period=30s \
  CMD curl -f http://localhost:8080/swagger/index.html || exit 1

ENTRYPOINT ["dotnet", "Identity.API.dll"]
