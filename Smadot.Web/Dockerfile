#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Smadot.Web/Smadot.Web.csproj", "Smadot.Web/"]
COPY ["Smadot.Models/Smadot.Models.csproj", "Smadot.Models/"]
COPY ["Utilities/Smadot.Utilities.csproj", "Utilities/"]
RUN dotnet restore "Smadot.Web/Smadot.Web.csproj"
COPY . .
WORKDIR "/src/Smadot.Web"
RUN dotnet build "Smadot.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Smadot.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Smadot.Web.dll"]