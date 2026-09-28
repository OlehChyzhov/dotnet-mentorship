FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8082
EXPOSE 8083

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["Airbnb.Audit.API/Airbnb.Audit.API.csproj", "Airbnb.Audit.API/"]
COPY ["Airbnb.Audit.Application/Airbnb.Audit.Application.csproj", "Airbnb.Audit.Application/"]
COPY ["Airbnb.Audit.Domain/Airbnb.Audit.Domain.csproj", "Airbnb.Audit.Domain/"]
RUN dotnet restore "Airbnb.Audit.API/Airbnb.Audit.API.csproj"

COPY . .
WORKDIR /src/Airbnb.Audit.API
RUN dotnet build "./Airbnb.Audit.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Airbnb.Audit.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Airbnb.Audit.API.dll"]
