#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Docker_api_med_dotNET.sln", "."]
COPY ["API_med_dotNET/API_med_dotNET.csproj", "API_med_dotNET/"]
RUN dotnet restore "API_med_dotNET/API_med_dotNET.csproj"
COPY . .
WORKDIR "/src/API_med_dotNET"
RUN dotnet build "API_med_dotNET.csproj" -c Release -o /app/build



FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "API_med_dotNET.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API_med_dotNET.dll"]
