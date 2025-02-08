using Microsoft.EntityFrameworkCore;
using APIFinal.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer("Server=.\\SQLEXPRESS;Database=SystemMonitoringDB;Trusted_Connection=True;TrustServerCertificate=True"),
    ServiceLifetime.Scoped);

void Configure(IServiceCollection services)
{
    services.AddCors(options =>
    {
        options.AddPolicy("AllowBlazorOrigin",
            builder =>
            {
                builder.WithOrigins("http://localhost:53555", "https://localhost:7054/");

            });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowBlazorOrigin");
app.UseCors(policy => policy.AllowAnyOrigin());

app.UseAuthorization();

app.MapControllers();

app.Run();
