FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DataGeneratorApi/DataGeneratorApi.csproj", "DataGeneratorApi/"]
RUN dotnet restore "DataGeneratorApi/DataGeneratorApi.csproj"
COPY . .
WORKDIR "/src/DataGeneratorApi"
RUN dotnet build "DataGeneratorApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataGeneratorApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataGeneratorApi.dll"]
