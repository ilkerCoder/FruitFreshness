using FruitFreshnessDetector.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(sp =>
{
    var modelPath = Path.Combine(Directory.GetCurrentDirectory(), "ML_Models", "resnet50_freshness.onnx");
    var resnetPath = Path.Combine(Directory.GetCurrentDirectory(), "ML_Models", "resnet50_freshness.onnx");
    var yoloPath = Path.Combine(Directory.GetCurrentDirectory(), "ML_Models", "yolov8n.onnx");
    return new DetectionService(resnetPath, yoloPath);
});
builder.Services.AddSingleton(sp =>
{
    var resnetPath = Path.Combine(Directory.GetCurrentDirectory(), "ML_Models", "resnet50_freshness.onnx");
    return new OnnxPredictionService(resnetPath);
});
builder.Services.AddCors();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //Sadece devde CORS kapat
    app.UseCors(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
    );
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
