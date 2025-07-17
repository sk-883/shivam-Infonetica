FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Point into the subfolder
COPY ["configurable-workflow-engine/configurable-workflow-engine.csproj", "./"]
RUN dotnet restore "configurable-workflow-engine.csproj"

# Copy everything from the subfolder
COPY configurable-workflow-engine/ ./

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS="http://+:8080"
EXPOSE 8080
ENTRYPOINT ["dotnet", "configurable-workflow-engine.dll"]
