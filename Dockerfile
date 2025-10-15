# -------------------------
# multi-stage build for .NET solution
# -------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy solution and project files to leverage layer cache
COPY *.sln ./
COPY Zenkoi.API/*.csproj ./Zenkoi.API/
COPY Zenkoi.BLL/*.csproj ./Zenkoi.BLL/
COPY Zenkoi.DAL/*.csproj ./Zenkoi.DAL/

# restore packages (uses the copied csproj files)
RUN dotnet restore

# copy everything and publish the API project
COPY . .
WORKDIR /src/Zenkoi.API
RUN dotnet publish -c Release -o /app/publish

# -------------------------
# runtime image
# -------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Fallback binding (Render provides PORT env var, default 10000)
ENV ASPNETCORE_URLS=http://+:10000

COPY --from=build /app/publish ./

# Use shell entry so we can honors Render's $PORT at runtime
ENTRYPOINT ["sh", "-lc", "export ASPNETCORE_URLS=http://0.0.0.0:${PORT:-10000} && exec dotnet Zenkoi.API.dll"]
