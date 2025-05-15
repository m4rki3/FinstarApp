FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FinstarApp/FinstarApp.csproj", "FinstarApp/"]
COPY ["FinstarApp.Infrastructure.Data/FinstarApp.Infrastructure.Data.csproj", "FinstarApp.Infrastructure.Data/"]
COPY ["FinstarApp.Domain.Core/FinstarApp.Domain.Core.csproj", "FinstarApp.Domain.Core/"]
COPY ["FinstarApp.Domain.Interfaces/FinstarApp.Domain.Interfaces.csproj", "FinstarApp.Domain.Interfaces/"]
COPY ["FinstarApp.Infrastructure.Business/FinstarApp.Infrastructure.Business.csproj", "FinstarApp.Infrastructure.Business/"]
COPY ["FinstarApp.Services.Interfaces/FinstarApp.Services.Interfaces.csproj", "FinstarApp.Services.Interfaces/"]
RUN dotnet restore "FinstarApp/FinstarApp.csproj"
COPY . .
WORKDIR "/src/FinstarApp"
RUN dotnet build "FinstarApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FinstarApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinstarApp.dll"]
