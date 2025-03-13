FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ReccomendationService/ReccomendationService.csproj", "ReccomendationService/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "ReccomendationService/ReccomendationService.csproj"
COPY . .
WORKDIR "/src/ReccomendationService"
RUN dotnet build "ReccomendationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReccomendationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReccomendationService.dll"]
