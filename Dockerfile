# ---------- BYGGEFASEN ----------

# Brug .NET SDK (med build-værktøjer) som base image til bygning
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiér projektfilen og gendan afhængigheder
COPY ["SEP4_User_Service.csproj", "./"]
RUN dotnet restore "SEP4_User_Service.csproj"

# Kopiér resten af kildekoden ind og publicer i Release-mode
COPY . . 
RUN dotnet restore "SEP4_User_Service.csproj"
RUN dotnet publish "SEP4_User_Service.csproj" -c Release -o /app/publish

# ---------- RUNTIME-FASEN ----------

# Brug det officielle ASP.NET runtime image til at køre appen
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Kopiér de færdigbyggede filer fra build-stage
COPY --from=build /app/publish .

# Eksponér porte til HTTP (5000) og gRPC (5001)
EXPOSE 5000
EXPOSE 5001

# Konfigurer ASP.NET til at lytte på HTTP (5000) — gRPC uden TLS kræver dette
ENV ASPNETCORE_URLS=http://+:5000

# Start applikationen
ENTRYPOINT ["dotnet", "SEP4_User_Service.dll"]
