FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FunkoAppi01/FunkoAppi01.csproj", "FunkoAppi01/"]
RUN dotnet restore "FunkoAppi01/FunkoAppi01.csproj"
COPY . .
WORKDIR "/src/FunkoAppi01"
RUN dotnet build "./FunkoAppi01.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FunkoAppi01.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FunkoAppi01.dll"]
