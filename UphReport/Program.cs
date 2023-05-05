using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using UphReport.Installers;
using UphReport.Interfaces;
using UphReport.Middleware;
using UphReport.Seeder;
using UphReport.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.InstallServicesInAssembly(builder.Configuration);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton<ErrorHandlingMiddleware>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<UphSeeder>();
builder.Services.AddScoped<IWebPage, WebPageService>();
builder.Services.AddScoped<IPageSpeedService, PageSpeedReporterService>();
builder.Services.AddScoped<IWaveAPIKeyService, WaveAPIKeyService>();
builder.Services.AddScoped<IWaveReporterService, WaveReporterService>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
