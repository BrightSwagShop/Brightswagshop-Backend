# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln ./
COPY FakeWebShop.Api/*.csproj ./FakeWebShop.Api/
COPY FakeWebShop.Contracts/*.csproj ./FakeWebShop.Contracts/
COPY FakeWebShop.Domain.Model/*.csproj ./FakeWebShop.Domain.Model/
COPY FakeWebShop.Domain.Services/*.csproj ./FakeWebShop.Domain.Services/
COPY FakeWebShop.Enums/*.csproj ./FakeWebShop.Enums/
COPY FakeWebShop.Persistence/*.csproj ./FakeWebShop.Persistence/
COPY FakeWebShop.Persistence.Entities/*.csproj ./FakeWebShop.Persistence.Entities/
COPY FakeWebShop.Persistence.Enums/*.csproj ./FakeWebShop.Persistence.Enums/
COPY FakeWebShop.Storage/*.csproj ./FakeWebShop.Storage/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Build and publish
WORKDIR /src/FakeWebShop.Api
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080

# Start the application
ENTRYPOINT ["dotnet", "FakeWebShop.Api.dll"]
