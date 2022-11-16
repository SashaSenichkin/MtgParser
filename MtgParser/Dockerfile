FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MtgParser/MtgParser.csproj", "MtgParser/"]
RUN dotnet restore "MtgParser/MtgParser.csproj"
COPY . .
WORKDIR "/src/MtgParser"
RUN dotnet build "MtgParser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MtgParser.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MtgParser.dll"]
