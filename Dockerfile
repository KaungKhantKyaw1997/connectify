# Use the .NET 9.0 ASP.NET runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Use the .NET 9.0 SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["Connectify.csproj", "./"]
RUN dotnet restore "Connectify.csproj"

# Copy the rest of the application code
COPY . .

# Build the application
WORKDIR "/src"
RUN dotnet build "Connectify.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Connectify.csproj" -c Release -o /app/publish

# Set the final image and copy the published files
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment to ensure Swagger works
ENV DOTNET_ENVIRONMENT Development

# Start the application
ENTRYPOINT ["dotnet", "Connectify.dll", "--urls", "http://0.0.0.0:80"]
