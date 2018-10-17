FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 44315

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Taxi/Taxi.csproj Taxi/
COPY TaxiCoinCoreLibrary/TaxiCoinCoreLibrary.csproj TaxiCoinCoreLibrary/
RUN dotnet restore Taxi/Taxi.csproj
COPY . .
WORKDIR /src/Taxi
RUN dotnet build Taxi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Taxi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Taxi.dll"]
