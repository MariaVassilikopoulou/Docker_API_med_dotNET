# Use .NET ASP.NET Core image for runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use .NET SDK image for build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the solution file and project file
COPY API_med_dotNET.sln .
COPY API_med_dotNET/API_med_dotNET.csproj API_med_dotNET/

# Restore dependencies
RUN dotnet restore "API_med_dotNET/API_med_dotNET.csproj"

# Copy the entire project directory
COPY . .

# Build and publish the project
WORKDIR "/src/API_med_dotNET"
RUN dotnet build "API_med_dotNET.csproj" -c ${BUILD_CONFIGURATION} -o /app/build
RUN dotnet publish "API_med_dotNET.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish

# Use the base image to create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "API_med_dotNET.dll"]
