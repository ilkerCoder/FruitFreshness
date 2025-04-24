# Temel image

# mcr.microsot.com bir sunucudur.  burdan aspnet image sini al ve base olarak adlandır diyor . 
# oradan su image cekilir : .NET 8.0 ASP.NET Runtime .  Microsoft Container registry den yani.
# Bu asp.net image sadece .dll ' lerin yürütülmesini saglar. kod yazmaz , derlemez.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Build image

# Burada bir sdk image ' ı  başlatıp , adını da build koyuyoruz. 
# .Net SDK içerir . Yani , dotnet build , restore gibi komutları çalıştırabilir.  
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
# Projedeki csproj dosyalarını okuyarak , gerekli nuget paketlerini indirir .
RUN dotnet restore
# Proje derlenir ve sonucu app/publish icine atar . 
RUN dotnet publish -c Release -o /app/publish

# Runtime image
# Runtime image
FROM base AS final
WORKDIR /app

# GDI+ desteği için libgdiplus kurulumu (bitmap, image vs için şart)
RUN apt-get update && \
    apt-get install -y libgdiplus && \
    [ -f /usr/lib/libgdiplus.so ] || ln -s /usr/lib/libgdiplus.so /usr/lib/libgdiplus.so

# Derlenmiş uygulamayı kopyala
COPY --from=build /app/publish ./
COPY ./Ml_Models ./Ml_Models

ENTRYPOINT ["dotnet", "FruitFreshnessDetector.dll"]
