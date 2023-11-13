#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ReservationApp/ReservationApp.csproj", "ReservationApp/"]
COPY ["Migrations/ReservationApp.LocalDb/ReservationApp.LocalDb.csproj", "Migrations/ReservationApp.LocalDb/"]
COPY ["ReservationApp.Models/ReservationApp.Models.csproj", "ReservationApp.Models/"]
COPY ["Migrations/ReservationApp.PostgreSql/ReservationApp.PostgreSql.csproj", "Migrations/ReservationApp.PostgreSql/"]
RUN dotnet restore "ReservationApp/ReservationApp.csproj"
COPY . .
WORKDIR "/src/ReservationApp"
RUN dotnet build "ReservationApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReservationApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReservationApp.dll"]