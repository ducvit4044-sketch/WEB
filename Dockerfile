FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["API TEST/API TEST.csproj", "API TEST/"]
RUN dotnet restore "API TEST/API TEST.csproj"
COPY . .
WORKDIR "/src/API TEST"
# Publish Self-Contained cho Linux x64
RUN dotnet publish "API TEST.csproj" -c Release -o /app/publish -r linux-x64 --self-contained true /p:PublishSingleFile=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "API TEST.dll"]