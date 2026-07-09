FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5088

ENV ASPNETCORE_URLS=http://+:5088
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


COPY ["LocalNuget/", "/src/LocalNuget/"]
COPY ["nuget.config", "/src/"]

COPY ["QSearchService/QSearchService.API/QSearchService.API.csproj", "QSearchService.API/"]
COPY ["QSearchService/QSearchService.Domain/QSearchService.Domain.csproj", "QSearchService.Domain/"]
COPY ["QSearchService/QSearchService.Infrastructure/QSearchService.Infrastructure.csproj", "QSearchService.Infrastructure/"]
COPY ["QSearchService/QSearchService.Application/QSearchService.Application.csproj", "QSearchService.Application/"]
RUN dotnet restore "QSearchService.API/QSearchService.API.csproj"
COPY . .
WORKDIR "/src/QSearchService/QSearchService.API"
RUN dotnet build "./QSearchService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./QSearchService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QSearchService.API.dll"]
