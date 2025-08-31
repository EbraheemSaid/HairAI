# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY HairAI.Api/*.csproj ./HairAI.Api/
COPY HairAI.Application/*.csproj ./HairAI.Application/
COPY HairAI.Domain/*.csproj ./HairAI.Domain/
COPY HairAI.Infrastructure/*.csproj ./HairAI.Infrastructure/
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish HairAI.Api -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "HairAI.Api.dll"]