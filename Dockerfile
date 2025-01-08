# Use the .NET 9.0 ASP.NET runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Use the .NET 9.0 SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Connectify.csproj", "Connectify/"]
RUN dotnet restore "Connectify/Connectify.csproj"
COPY . .
WORKDIR "/src/Connectify"
RUN dotnet build "Connectify.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "Connectify.csproj" -c Release -o /app/publish

# Set the final image and copy the published files
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Connectify.dll"]
