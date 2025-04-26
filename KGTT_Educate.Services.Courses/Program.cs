using KGTT_Educate.Services.Courses.Data;
using KGTT_Educate.Services.Courses.Data.Interfaces;
using KGTT_Educate.Services.Courses.Utils;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mapsterConfig = new TypeAdapterConfig();
builder.Services.AddSingleton(mapsterConfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoConnection"));

builder.Services.AddSingleton<IMongoDatabase>(provider =>
{
    var mongoSettings = provider.GetService<IOptions<MongoSettings>>().Value;
    var client = new MongoClient(mongoSettings.ConnectionString);
    return client.GetDatabase(mongoSettings.Database);
});

builder.Services.Configure<FileStorageSettings>(builder.Configuration.GetSection("FileStorage"));
builder.Services.Configure<MediaStorageSettings>(builder.Configuration.GetSection("MediaStorage"));

// Регистрация сервиса для файлов
builder.Services.AddScoped<IFileService>(provider =>
{
    var settings = provider.GetService<IOptions<FileStorageSettings>>().Value;
    var fileStoragePath = Path.Combine(Directory.GetCurrentDirectory(), settings.Path);

    if (!Directory.Exists(fileStoragePath))
    {
        Directory.CreateDirectory(fileStoragePath);
    }

    return new FileService(fileStoragePath);
});

builder.Services.AddScoped<ICourseService, CourseService>(); 
builder.Services.AddScoped<ICourseMediaService, CourseMediaService>(); 
builder.Services.AddScoped<ICourseFileService, CourseFileService>(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
    await next.Invoke();
});

app.UseRouting();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
