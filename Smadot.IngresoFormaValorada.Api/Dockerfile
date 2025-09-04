#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Smadot.IngresoFormaValorada.Api/Smadot.IngresoFormaValorada.Api.csproj", "Smadot.IngresoFormaValorada.Api/"]
COPY ["Smadot.IngresoFormaValorada.Model/Smadot.IngresoFormaValorada.Model.csproj", "Smadot.IngresoFormaValorada.Model/"]
COPY ["Smadot.Models/Smadot.Models.csproj", "Smadot.Models/"]
COPY ["Utilities/Smadot.Utilities.csproj", "Utilities/"]
COPY ["Smadot.SeguimientoCVV.Model/Smadot.SeguimientoCVV.Model.csproj", "Smadot.SeguimientoCVV.Model/"]

COPY ["Smadsot.Historico.Models/Smadsot.Historico.Models.csproj", "Smadsot.Historico.Models/"]
RUN dotnet restore "Smadot.IngresoFormaValorada.Api/Smadot.IngresoFormaValorada.Api.csproj"
COPY . .
WORKDIR "/src/Smadot.IngresoFormaValorada.Api"
RUN dotnet build "Smadot.IngresoFormaValorada.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Smadot.IngresoFormaValorada.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Smadot.IngresoFormaValorada.Api.dll"]