FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app
ENV UPDATE_INTERVAL="60"
ENV LANGUAGE="en-US"
ENV DEBUG=0

# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Discord-Status-Bot.csproj", "."]
RUN dotnet restore "./Discord-Status-Bot.csproj"
COPY . .
WORKDIR /src/.
RUN dotnet build "./Discord-Status-Bot.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Discord-Status-Bot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Release
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Discord-Status-Bot.dll"]