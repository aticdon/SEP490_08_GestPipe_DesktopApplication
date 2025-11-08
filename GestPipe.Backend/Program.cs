using GestPipe.Backend.Models;
using GestPipe.Backend.Services;
using GestPipe.Backend.Services.IServices;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GestPipe.Backend.Config.I18nConfig>(builder.Configuration.GetSection("I18n"));

builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration["MongoDB:ConnectionString"]));

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(builder.Configuration["MongoDB:Database"]);
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDefaultGestureService, DefaultGestureService>();
builder.Services.AddScoped<IGestureTypeService, GestureTypeService>();
builder.Services.AddScoped<IUserGestureConfigService, UserGestureConfigService>();
builder.Services.AddScoped<ITrainingGestureService, TrainingGestureService>();
// Add Services
//builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<TopicService>();
builder.Services.AddSingleton<SessionService>();
builder.Services.AddSingleton<CategoryService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();
app.Run();