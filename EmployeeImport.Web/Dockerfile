FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EmployeeImport.Web/EmployeeImport.Web.csproj", "EmployeeImport.Web/"]
COPY ["EmployeeImport.Infrastructure/EmployeeImport.Infrastructure.csproj", "EmployeeImport.Infrastructure/"]
COPY ["EmployeeImport.Application/EmployeeImport.Application.csproj", "EmployeeImport.Application/"]
COPY ["EmployeeImport.Domain/EmployeeImport.Domain.csproj", "EmployeeImport.Domain/"]
RUN dotnet restore "EmployeeImport.Web/EmployeeImport.Web.csproj"
COPY . .
WORKDIR "/src/EmployeeImport.Web"
RUN dotnet build "EmployeeImport.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EmployeeImport.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmployeeImport.Web.dll"]
