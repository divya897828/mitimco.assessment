﻿using Microsoft.OpenApi.Models;
using mitimco.assessment.Models;
using mitimco.assessment.Repositories;
using mitimco.assessment.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddSwaggerGen(c =>
{
    // This will create Swagger docs for your API version
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Stock API", Version = "v1" });

    // Ensure this path is correct, and points to the XML file generated by your project
    var xmlFile = Path.Combine(AppContext.BaseDirectory, "mitimco.assessment.xml");
    c.IncludeXmlComments(xmlFile);
});
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddHttpClient<IStockRepository, StockRepository>();

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock API V1");
    c.RoutePrefix = string.Empty; // Makes Swagger UI available at the root URL (e.g., http://localhost:5000/)
});

app.MapControllers();

app.Run();

