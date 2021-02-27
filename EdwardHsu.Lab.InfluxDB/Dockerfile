#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["EdwardHsu.Lab.InfluxDB/EdwardHsu.Lab.InfluxDB.csproj", "EdwardHsu.Lab.InfluxDB/"]
RUN dotnet restore "EdwardHsu.Lab.InfluxDB/EdwardHsu.Lab.InfluxDB.csproj"
COPY . .
WORKDIR "/src/EdwardHsu.Lab.InfluxDB"
RUN dotnet build "EdwardHsu.Lab.InfluxDB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EdwardHsu.Lab.InfluxDB.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EdwardHsu.Lab.InfluxDB.dll"]