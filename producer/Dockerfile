FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

COPY producer.csproj .
RUN dotnet restore producer.csproj

COPY . .
RUN dotnet publish producer.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=build /app .
CMD ["dotnet", "producer.dll"]
