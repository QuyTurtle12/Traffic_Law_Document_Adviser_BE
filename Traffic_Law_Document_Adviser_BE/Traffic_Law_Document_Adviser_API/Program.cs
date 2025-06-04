using DataAccess.DTOs.NewsDTOs;
using DataAccess.Entities;
using DataAccess.IRepositories;
using DataAccess.IServices;
using DataAccess.Repositories;
using DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Product_Sale_API.Middleware;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

// Register Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// Get connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("MyCnn");

// Register DbContext with DI
builder.Services.AddDbContext<TrafficLawDocumentDbContext>(options =>
    options.UseSqlServer(connectionString));

// Make all route lowercase
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

// Add Swagger services with XML comments
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Swagger API",
        Version = "v1"
    });

    // Add XML Comments
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Register AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Register Repositories and Services
builder.Services.AddScoped<IUOW, UOW>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<INewsService, NewsService>();

builder.Services.AddAutoMapper(config =>
{
    config.CreateMap<News, GetNewsDTO>();
    config.CreateMap<AddNewsDTO, News>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseAuthorization();

//app.UseMiddleware<PermissionHandlingMiddleware>();

app.UseMiddleware<CustomExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();
