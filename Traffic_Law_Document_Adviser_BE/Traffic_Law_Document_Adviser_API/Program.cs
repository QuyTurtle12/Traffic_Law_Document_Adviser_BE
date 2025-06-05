using BusinessLogic.IServices;
using BusinessLogic.Services;
using DataAccess.Constant;
using DataAccess.DTOs;
using DataAccess.DTOs.AuthDTOs;
using DataAccess.Entities;
using DataAccess.IRepositories;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Product_Sale_API.Middleware;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Bind JwtSettings from appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Add Authentication + JWT Bearer
var jwtSection = builder.Configuration.GetSection("JwtSettings");
var jwtConfig = jwtSection.Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig.Issuer,
        ValidAudience = jwtConfig.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(jwtConfig.Key))
    };
});
// 3.1. Configure role‐based policies
builder.Services.AddAuthorization(options =>
{
    // Only Admin can do certain things
    options.AddPolicy("RequireAdminRole", policy =>
      policy.RequireRole(RoleConstants.Admin));

    // Expert OR Admin
    options.AddPolicy("RequireExpertOrAdmin", policy =>
      policy.RequireRole(RoleConstants.Expert, RoleConstants.Admin));

    // Any authenticated User (including Expert/Admin)
    options.AddPolicy("RequireAnyUserRole", policy =>
      policy.RequireRole(RoleConstants.User, RoleConstants.Expert, RoleConstants.Admin));
});

// Register Repositories and Services
builder.Services.AddScoped<IUOW, UOW>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<PermissionHandlingMiddleware>();

app.UseMiddleware<CustomExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();
