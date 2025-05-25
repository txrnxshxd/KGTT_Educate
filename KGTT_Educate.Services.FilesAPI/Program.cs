using KGTT_Educate.Services.FilesAPI.Data.Interfaces.Services;
using KGTT_Educate.Services.FilesAPI.Data.Services;
using KGTT_Educate.Services.FilesAPI.Utils;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

// Регистрация сервиса для файлов
builder.Services.Configure<FileStorage>(builder.Configuration.GetSection("FileStorage"));

builder.Services.AddScoped<IFileService>(provider =>
{
    return new FileService(builder.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(builder => builder.AllowAnyOrigin()
                                  .AllowAnyOrigin()
                                  .AllowAnyHeader());
}

app.UseAuthorization();

app.MapControllers();

app.Run();
