FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["DatabaseManagementService/DatabaseManagementService.csproj", "DatabaseManagementService/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "DatabaseManagementService/DatabaseManagementService.csproj"
COPY . .
WORKDIR "/src/DatabaseManagementService"
RUN dotnet build "DatabaseManagementService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DatabaseManagementService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatabaseManagementService.dll"]
