FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["StocksProcessor/StocksProcessor.csproj", "StocksProcessor/"]
RUN dotnet restore "StocksProcessor/StocksProcessor.csproj"
COPY . .
WORKDIR "/src/StocksProcessor"
RUN dotnet build "StocksProcessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StocksProcessor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StocksProcessor.dll"]
