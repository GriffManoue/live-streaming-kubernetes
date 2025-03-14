FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["RecommendationService/RecommendationService.csproj", "RecommendationService/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "RecommendationService/RecommendationService.csproj"
COPY . .
WORKDIR "/src/RecommendationService"
RUN dotnet build "RecommendationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RecommendationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecommendationService.dll"]
