#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Smadsot.FotoMultaService.Api/Smadsot.FotoMultaService.Api.csproj", "Smadsot.FotoMultaService.Api/"]
COPY ["Smadsot.FotoMulta.Model/Smadsot.FotoMulta.Model.csproj", "Smadsot.FotoMulta.Models/"]
COPY ["Smadsot.FotoMulta.Negocio/Smadsot.FotoMulta.Negocio.csproj", "Smadsot.FotoMulta.Negocio/"]
RUN dotnet restore "Smadsot.FotoMultaService.Api/Smadsot.FotoMultaService.Api.csproj"
COPY . .
WORKDIR "/src/Smadsot.FotoMultaService.Api"
RUN dotnet build "Smadsot.FotoMultaService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Smadsot.FotoMultaService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Smadsot.FotoMultaService.Api.dll"]