using KGTT_Educate.Services.Courses.Data.Interfaces.Repository;
using KGTT_Educate.Services.Courses.Data.Interfaces.UoW;
using KGTT_Educate.Services.Courses.Data.Repository;
using KGTT_Educate.Services.Courses.Data.UoW;
using KGTT_Educate.Services.Courses.SyncDataServices.Http;
using KGTT_Educate.Services.Courses.Utils;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Cryptography.X509Certificates;

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


//builder.Services.AddScoped<ILessonsRepository>(provider =>

//    new LessonsRepository(
//        provider.GetRequiredService<IMongoDatabase>()
//    )
//);

//builder.Services.AddScoped<ILessonFilesRepository>(provider =>
//    new LessonFilesRepository(
//        provider.GetRequiredService<IMongoDatabase>()
//    )
//);

//builder.Services.AddScoped<ICourseFilesRepository>(provider =>
//    new CourseFilesRepository(
//        provider.GetRequiredService<IMongoDatabase>()
//    )
//);

//builder.Services.AddScoped<ICoursesRepository>(provider =>
//    new CoursesRepository(
//        provider.GetRequiredService<IMongoDatabase>()
//    )
//);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(10001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        listenOptions.UseHttps(httpsOptions =>
        {
            httpsOptions.ServerCertificate = new X509Certificate2(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".aspnet",
                "https",
                "kgttedu.pfx"
                ),
                "txrnxshxd!"
            );
        });
    });
});

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
