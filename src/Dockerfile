#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app
ENV UPDATE_INTERVAL="60"
ENV LANGUAGE="en-US"
ENV DEBUG=0

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["Discord-Status-Bot/Discord-Status-Bot.csproj", "Discord-Status-Bot/"]
RUN dotnet restore "./Discord-Status-Bot/Discord-Status-Bot.csproj"
COPY . .
WORKDIR "/src/Discord-Status-Bot"
RUN dotnet build "./Discord-Status-Bot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Discord-Status-Bot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Discord-Status-Bot.dll"]