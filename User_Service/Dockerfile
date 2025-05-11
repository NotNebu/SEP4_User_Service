# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy p12 certifikat og kode
COPY --from=build /app/publish .
COPY ./certs/localhost-user-service.p12 /certs/localhost-user-service.p12

# Kør med HTTPS på port 5001
EXPOSE 5001
ENTRYPOINT ["dotnet", "User_Service.dll"]
