FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


COPY API_med_dotNET.sln .
COPY API_med_dotNET/API_med_dotNET.csproj API_med_dotNET/


RUN dotnet restore "API_med_dotNET/API_med_dotNET.csproj"

COPY . .


WORKDIR "/src/API_med_dotNET"
RUN dotnet build "API_med_dotNET.csproj" -c ${BUILD_CONFIGURATION} -o /app/build
RUN dotnet publish "API_med_dotNET.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "API_med_dotNET.dll"]
