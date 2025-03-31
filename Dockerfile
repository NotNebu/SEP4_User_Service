# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SEP4_User_Service.csproj", "./"]
RUN dotnet restore "SEP4_User_Service.csproj"

COPY . .
RUN dotnet publish "SEP4_User_Service.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000
EXPOSE 5001

# Tvinger ASP.NET til at lytte på port 5000 (HTTP) ellers fejler gRPC
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "SEP4_User_Service.dll"]
