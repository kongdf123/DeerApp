# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
#FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
#
## This stage is used to build the service project
#FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["Order.API/Order.API.csproj", "Order.API/"]
#RUN dotnet restore "./Order.API/Order.API.csproj"
#COPY . .
#WORKDIR "/src/Order.API"
#RUN dotnet build "./Order.API.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
## This stage is used to publish the service project to be copied to the final stage
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./Order.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
## This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Order.API.dll"]

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["services/order/Order.API/Order.API.csproj", "services/order/Order.API/"]
COPY ["services/order/Order.Application/Order.Application.csproj", "services/order/Order.Application/"]
COPY ["services/order/Order.Domain/Order.Domain.csproj", "services/order/Order.Domain/"]
COPY ["services/order/Order.Infrastructure/Order.Infrastructure.csproj", "services/order/Order.Infrastructure/"]

COPY ["shared/Shared.Contracts/Shared.Contracts.csproj", "shared/Shared.Contracts/"]

RUN dotnet restore "services/order/Order.API/Order.API.csproj"

COPY . .

WORKDIR "/src/services/order/Order.API"

RUN dotnet publish "Order.API.csproj" \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Order.API.dll"]