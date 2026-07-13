# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first — restores are cached until .csproj files change
COPY src/MyModularStore.Shared/MyModularStore.Shared.csproj src/MyModularStore.Shared/
COPY src/MyModularStore.Orders/MyModularStore.Orders.csproj  src/MyModularStore.Orders/

RUN dotnet restore src/MyModularStore.Orders/MyModularStore.Orders.csproj

# Copy source code and publish
COPY src/MyModularStore.Shared/ src/MyModularStore.Shared/
COPY src/MyModularStore.Orders/ src/MyModularStore.Orders/

RUN dotnet publish src/MyModularStore.Orders/MyModularStore.Orders.csproj \
    -c Release -o /app/publish --no-restore

# ── Stage 2: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MyModularStore.Orders.dll"]
