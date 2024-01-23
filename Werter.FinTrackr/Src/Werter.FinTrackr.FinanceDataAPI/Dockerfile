FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Src/Werter.FinTrackr.FinanceDataAPI/Werter.FinTrackr.FinanceDataAPI.csproj", "Src/Werter.FinTrackr.FinanceDataAPI/"]
RUN dotnet restore "Src/Werter.FinTrackr.FinanceDataAPI/Werter.FinTrackr.FinanceDataAPI.csproj"
COPY . .
WORKDIR "/src/Src/Werter.FinTrackr.FinanceDataAPI"
RUN dotnet build "Werter.FinTrackr.FinanceDataAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Werter.FinTrackr.FinanceDataAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Werter.FinTrackr.FinanceDataAPI.dll"]
