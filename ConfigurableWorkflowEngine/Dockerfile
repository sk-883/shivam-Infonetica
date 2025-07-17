# 1) Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ConfigurableWorkflowEngine.csproj", "./"]
RUN dotnet restore "ConfigurableWorkflowEngine.csproj"

COPY . .
RUN dotnet publish "ConfigurableWorkflowEngine.csproj" \
    -c Release \
    -o /app/publish

# 2) Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS="http://+:8080"
EXPOSE 8080

ENTRYPOINT ["dotnet", "ConfigurableWorkflowEngine.dll"]
