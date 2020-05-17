
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY QueryMasterCore/*.csproj ./QueryMasterCore/
COPY MayhemBot/*.csproj ./MayhemBot/
WORKDIR /app/MayhemBot
RUN dotnet restore

# Copy everything else and build
WORKDIR /app
COPY QueryMasterCore/. ./QueryMasterCore/
COPY MayhemBot/. ./MayhemBot/
WORKDIR /app/MayhemBot
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/MayhemBot/out .
ENTRYPOINT ["dotnet", "MayhemDiscord.Bot.dll"]
