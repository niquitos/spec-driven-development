FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TaskTracker.sln", "."]
COPY ["src/TaskTracker.Api/TaskTracker.Api.csproj", "src/TaskTracker.Api/"]
COPY ["src/TaskTracker.Application/TaskTracker.Application.csproj", "src/TaskTracker.Application/"]
COPY ["src/TaskTracker.Domain/TaskTracker.Domain.csproj", "src/TaskTracker.Domain/"]
COPY ["src/TaskTracker.Infrastructure/TaskTracker.Infrastructure.csproj", "src/TaskTracker.Infrastructure/"]
RUN dotnet restore "src/TaskTracker.Api/TaskTracker.Api.csproj"

COPY . .
WORKDIR "/src/src/TaskTracker.Api"
RUN dotnet build "TaskTracker.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskTracker.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskTracker.Api.dll"]
