FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

# The restore operation (Pulling all of the nuget packages) is expensive.
# We should run that operation only if new packages were installed, not on any code change.
# To do that we copy the .csproj files that have the refferences to the nuget packages
# And run restore only if those files changed
WORKDIR /src
COPY ["Airbnb.API/Airbnb.API.csproj", "Airbnb.API/"]
COPY ["Airbnb.Infrastructure/Airbnb.Infrastructure.csproj", "Airbnb.Infrastructure/"]
COPY ["Airbnb.Application/Airbnb.Application.csproj", "Airbnb.Application/"]
COPY ["Airbnb.Domain/Domain.API.csproj", "Airbnb.Domain/"]
RUN dotnet restore "Airbnb.API/Airbnb.API.csproj"

# In here we copy the rest of the code, so on any change the execution of the build of 
# the new image starts only from here
COPY . .
WORKDIR /src/Airbnb.API
RUN dotnet build "Airbnb.API.csproj" -c BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Airbnb.API.csproj" -c BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app

# By default COPY copies from my local machine's build context. 
# The --from=build instead copies from another stage's filesystem
COPY --from=publish /app/publish .

# The command copies from the publish files that are inside /app/publish into the /app folder build 
# at stage 1 that contains the runtime of the .net

# This is the equivalent to dotnet Airbnb.API.dll command that runs the application.
# Everything before has been running on build time of the image. This is what happens in the end
# In the end we just run the application, so that's exactly it 
ENTRYPOINT ["dotnet", "Airbnb.API.dll"]