#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Smadot.Autenticacion.api/Smadot.Autenticacion.api.csproj", "Smadot.Autenticacion.api/"]
COPY ["Smadot.Models/Smadot.Models.csproj", "Smadot.Models/"]
COPY ["Utilities/Smadot.Utilities.csproj", "Utilities/"]
COPY ["Smadot.Autenticacion.Model/Smadot.Autenticacion.Model.csproj", "Smadot.Autenticacion.Model/"]
RUN dotnet restore "Smadot.Autenticacion.api/Smadot.Autenticacion.api.csproj"
COPY . .
WORKDIR "/src/Smadot.Autenticacion.api"
RUN dotnet build "Smadot.Autenticacion.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Smadot.Autenticacion.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Smadot.Autenticacion.api.dll"]