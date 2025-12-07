# syntax=docker/dockerfile:1.6
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY PhysicsProject.sln ./
COPY PhysicsProject.Core/PhysicsProject.Core.csproj PhysicsProject.Core/
COPY PhysicsProject.Application/PhysicsProject.Application.csproj PhysicsProject.Application/
COPY PhysicsProject.Infrastructure/PhysicsProject.Infrastructure.csproj PhysicsProject.Infrastructure/
COPY PhysicsProject.Api/PhysicsProject.Api.csproj PhysicsProject.Api/
RUN dotnet restore PhysicsProject.sln
COPY . ./
RUN dotnet publish PhysicsProject.Api/PhysicsProject.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PhysicsProject.Api.dll"]
