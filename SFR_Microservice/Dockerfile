FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SFR_Microservice/SFR_Microservice.csproj", "SFR_Microservice/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["SFRDataAccess/SFRDataAccess.csproj", "SFRDataAccess/"]
RUN dotnet restore "SFR_Microservice/SFR_Microservice.csproj"
COPY . .
WORKDIR "/src/SFR_Microservice"
RUN dotnet build "SFR_Microservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SFR_Microservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SFR_Microservice.dll"]
