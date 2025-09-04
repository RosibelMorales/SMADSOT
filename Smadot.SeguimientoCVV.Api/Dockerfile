#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Smadot.SeguimientoCVV.Api/Smadot.SeguimientoCVV.Api.csproj", "Smadot.SeguimientoCVV.Api/"]
COPY ["Smadot.SeguimientoCVV.Model/Smadot.SeguimientoCVV.Model.csproj", "Smadot.SeguimientoCVV.Model/"]
COPY ["Smadot.Models/Smadot.Models.csproj", "Smadot.Models/"]
COPY ["Utilities/Smadot.Utilities.csproj", "Utilities/"]
RUN dotnet restore "Smadot.SeguimientoCVV.Api/Smadot.SeguimientoCVV.Api.csproj"
COPY . .
WORKDIR "/src/Smadot.SeguimientoCVV.Api"
RUN dotnet build "Smadot.SeguimientoCVV.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Smadot.SeguimientoCVV.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Smadot.SeguimientoCVV.Api.dll"]