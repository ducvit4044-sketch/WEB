# Giai đoạn 1: Build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Vì tên project của bạn có dấu cách ("API TEST.csproj"), bắt buộc phải để trong ngoặc kép như thế này:
COPY ["API TEST/API TEST.csproj", "API TEST/"]
RUN dotnet restore "API TEST/API TEST.csproj"
COPY . .
WORKDIR "/src/API TEST"
RUN dotnet publish "API TEST.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Giai đoạn 2: Chạy ứng dụng trên Cloud
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "API TEST.dll"]