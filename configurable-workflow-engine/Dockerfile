# ---- Build stage ----
    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src
    
    # Copy csproj and restore dependencies
    COPY ["configurable-workflow-engine.csproj", "./"]
    RUN dotnet restore
    
    # Copy everything else and publish
    COPY . .
    RUN dotnet publish -c Release -o /app/publish
    
    # ---- Runtime stage ----
    FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
    WORKDIR /app
    
    # Listen on port 8080 (Cloud Run default)
    ENV ASPNETCORE_URLS=http://+:8080
    
    # Copy published output
    COPY --from=build /app/publish .
    
    # Expose port
    EXPOSE 8080
    
    # Launch the app
    ENTRYPOINT ["dotnet", "configurable-workflow-engine.dll"]
    