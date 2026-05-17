FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["OnlineLibrary.Api/OnlineLibrary.Api.csproj", "OnlineLibrary.Api/"]
COPY ["OnlineLibrary.Application/OnlineLibrary.Application.csproj", "OnlineLibrary.Application/"]
COPY ["OnlineLibrary.Domain/OnlineLibrary.Domain.csproj", "OnlineLibrary.Domain/"]
COPY ["OnlineLibrary.Persistence/OnlineLibrary.Persistence.csproj", "OnlineLibrary.Persistence/"]

RUN dotnet restore "OnlineLibrary.Api/OnlineLibrary.Api.csproj"

COPY . .

RUN dotnet publish "OnlineLibrary.Api/OnlineLibrary.Api.csproj" -c Release -o /app/publish

FROM base AS final

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "OnlineLibrary.Api.dll"]