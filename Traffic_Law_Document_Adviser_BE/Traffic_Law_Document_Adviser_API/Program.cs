using DataAccess.DTOs.NewsDTOs;
using DataAccess.Entities;
using Product_Sale_API.Middleware;
using Traffic_Law_Document_Adviser_API.DI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddInfrastructure(builder.Configuration);

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

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CustomExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();
