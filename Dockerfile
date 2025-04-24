# Temel .NET ASP.NET 8.0 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Build aşaması: SDK ile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Tüm proje dosyalarını kopyala ve restore + publish
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Final image: sadece runtime içeren
FROM base AS final
WORKDIR /app

# Derlenmiş uygulamayı ve model klasörünü kopyala
COPY --from=build /app/publish ./
COPY ./Fonts ./Fonts
COPY ./Ml_Models ./Ml_Models

# Uygulama giriş noktası
ENTRYPOINT ["dotnet", "FruitFreshnessDetector.dll"]
