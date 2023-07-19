FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ProcessorApp/ProcessorApp.csproj", "ProcessorApp/"]
RUN dotnet restore "ProcessorApp/ProcessorApp.csproj"
COPY . .
WORKDIR "/src/ProcessorApp"
RUN dotnet build "ProcessorApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProcessorApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProcessorApp.dll"]
