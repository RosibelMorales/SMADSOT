#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Smadot.SmadotTareas.Api/Smadot.SmadotTareas.Api.csproj", "Smadot.SmadotTareas.Api/"]
COPY ["Smadot.Models/Smadot.Models.csproj", "Smadot.Models/"]
COPY ["Smadsot.Historico.Models/Smadsot.Historico.Models.csproj", "Smadsot.Historico.Models/"]
COPY ["Utilities/Smadot.Utilities.csproj", "Utilities/"]
COPY ["Smadot.SmadotTareas.Negocio/Smadot.SmadotTareas.Negocio.csproj", "Smadot.SmadotTareas.Negocio/"]
RUN dotnet restore "Smadot.SmadotTareas.Api/Smadot.SmadotTareas.Api.csproj"
COPY . .
WORKDIR "/src/Smadot.SmadotTareas.Api"
RUN dotnet build "Smadot.SmadotTareas.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Smadot.SmadotTareas.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Smadot.SmadotTareas.Api.dll"]