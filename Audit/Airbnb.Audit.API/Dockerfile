FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8082
EXPOSE 8083

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["Audit/Airbnb.Audit.API/Airbnb.Audit.API.csproj", "Audit/Airbnb.Audit.API/"]
COPY ["Audit/Airbnb.Audit.Application/Airbnb.Audit.Application.csproj", "Audit/Airbnb.Audit.Application/"]
COPY ["Audit/Airbnb.Audit.Infrastructure/Airbnb.Audit.Infrastructure.csproj", "Audit/Airbnb.Audit.Infrastructure/"]
COPY ["Audit/Airbnb.Audit.Domain/Airbnb.Audit.Domain.csproj", "Audit/Airbnb.Audit.Domain/"]
COPY ["Shared/Airbnb.Messaging/Airbnb.Messaging.csproj", "Shared/Airbnb.Messaging/"]
COPY ["Shared/Airbnb.Contracts/Airbnb.Contracts.csproj", "Shared/Airbnb.Contracts/"]
RUN dotnet restore "Audit/Airbnb.Audit.API/Airbnb.Audit.API.csproj"

COPY . .
WORKDIR /src/Audit/Airbnb.Audit.API
RUN dotnet build "Airbnb.Audit.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Airbnb.Audit.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Airbnb.Audit.API.dll"]
