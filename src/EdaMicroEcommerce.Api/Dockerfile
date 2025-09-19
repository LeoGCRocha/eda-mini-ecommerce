FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/EdaMicroEcommerce.Api/EdaMicroEcommerce.Api.csproj", "src/EdaMicroEcommerce.Api/"]
COPY ["src/EdaMicroEcommerce.Application/EdaMicroEcommerce.Application.csproj", "src/EdaMicroEcommerce.Application/"]
COPY ["EdaMicroEcommerce.Domain/EdaMicroEcommerce.Domain.csproj", "EdaMicroEcommerce.Domain/"]
COPY ["src/EdaMicroEcommerce.Infra/EdaMicroEcommerce.Infra.csproj", "src/EdaMicroEcommerce.Infra/"]
RUN dotnet restore "src/EdaMicroEcommerce.Api/EdaMicroEcommerce.Api.csproj"
COPY . .
WORKDIR "/src/src/EdaMicroEcommerce.Api"
RUN dotnet build "./EdaMicroEcommerce.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EdaMicroEcommerce.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EdaMicroEcommerce.Api.dll"]
