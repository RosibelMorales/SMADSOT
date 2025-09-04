#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Smadot.Venta.Api/Smadot.Venta.Api.csproj", "Smadot.Venta.Api/"]
COPY ["Smadot.Models/Smadot.Models.csproj", "Smadot.Models/"]
COPY ["Utilities/Smadot.Utilities.csproj", "Utilities/"]
COPY ["Smadot.Venta.Model/Smadot.Venta.Model.csproj", "Smadot.Venta.Model/"]
COPY ["Smadot.Venta.Api/OpenApis/VentaApi.json", "/src/OpenApis/VentaApi.json"]
RUN dotnet restore "Smadot.Venta.Api/Smadot.Venta.Api.csproj"
COPY . .
WORKDIR "/src/Smadot.Venta.Api"
RUN dotnet build "Smadot.Venta.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Smadot.Venta.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Smadot.Venta.Api.dll"]