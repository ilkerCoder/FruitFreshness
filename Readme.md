FruitFreshnessDetector

FruitFreshnessDetector, bir kullanıcının görsel yüklenerek içindeki elmaların sağlıklı veya çürük olup olmadığını belirleyebildiği yapay zeka destekli bir uygulamadır. Arka planda YOLOv8 tespiti ve ResNet50 tabanlı ONNX modeli ile sınıflandırma yapar. Frontend tarafı Angular ile geliştirilmiştir.

✅ İçerik

Özellikler

Klasör Yapısı

Kurulum

Backend Mimari

Frontend Mimari

Kullanım

API Örneği

Ekran Görüntüsü

✨ Özellikler



📂 Klasör Yapısı

FruitFreshnessDetector/         <-- ASP.NET Core Web API
├── Controllers/
├── Services/
├── Models/resnet50_freshness.onnx
├── Models/yolov8n.onnx
├── Uploads/
├── Outputs/
├── Program.cs

FruitFreshness.App/             <-- Angular Frontend
├── src/
│   └── app/
│       └── components/
│           └── freshness-card/

🚀 Kurulum

Backend (ASP.NET)

cd FruitFreshnessDetector
# ONNX modellerini "Models/" klasörüne koyun
# Ardından projeyi Visual Studio veya CLI ile başlatın

Frontend (Angular)

cd FruitFreshness.App
npm install
npm start  # http://localhost:5000

🏛️ Backend Mimari

DetectionService.cs

YOLOv8 (.onnx) ile elma kutuları tespit edilir.

Her kutu ResNet50 ile "Healthy" veya "Rotten" olarak sınıflandırılır.

Annotated görsel Outputs/result.jpg olarak kaydedilir.

Endpoint

POST /api/detection

Çıktı:

{
  "healthyCount": 2,
  "rottenCount": 1,
  "annotatedImageBase64": "data:image/jpeg;base64,..."
}

📂 Frontend Mimari

Component: FreshnessCard

Kullanıcıdan görsel alır (file input)

api/detection'a POST atar

Gelen sonucu ekranda listeler + resmi gösterir

Bootstrap kullanılır, kart ortalanmıştır

📝 Kullanım

Angular frontend başlatılır localhost:5000

Kullanıcı bir elma görseli yükler

Sistem resmi Uploads/ dizinine kaydeder

YOLOv8 ile elmalar bulunur

Her elma ResNet50 ile sınıflandırılır

Annotated görsel gösterilir, sayılarla birlikte

📊 API Örneği (Postman / Swagger)

Endpoint: POST https://localhost:3000/api/detection

Header: Content-Type: multipart/form-data

Body: image = [dosya seçin]

📺 Ekran Görüntüsü



  

🚫 Uyarılar

Bu proje sadece geliştirme/test amacılıdır

Model tahminleri %100 doğruluk garantisi vermez

Hazırlayan: Fatih Metiner Proje: Yüksek Lisans Bitirme Projesi - 2025

