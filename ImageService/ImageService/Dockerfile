FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ImageService/ImageService/ImageService.csproj", "ImageService/ImageService/"]
RUN dotnet restore "ImageService/ImageService/ImageService.csproj"
COPY . .
WORKDIR "/src/ImageService/ImageService"
RUN dotnet build "ImageService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ImageService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImageService.dll"]
