using KGTT_Educate.Services.Events.Data;
using KGTT_Educate.Services.Events.Data.Repository;
using KGTT_Educate.Services.Events.Data.Repository.Interfaces;
using KGTT_Educate.Services.Events.SyncDataServices.Grpc;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddGrpc();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var mapsterConfig = new TypeAdapterConfig();
builder.Services.AddSingleton(mapsterConfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();
builder.Services.AddScoped<IGrpcAccountClient, GrpcAccountClient>();

builder.Services.AddScoped<IUoW, UoW>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(10003, listenOptions =>
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
    app.UseCors(builder => builder.AllowAnyOrigin()
                                  .AllowAnyHeader());
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
