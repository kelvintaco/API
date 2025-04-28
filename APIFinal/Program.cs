using Microsoft.EntityFrameworkCore;
using APIFinal.Context;
using OfficeOpenXml;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer("Server=.\\SQLEXPRESS;Database=SystemDB;Trusted_Connection=True;TrustServerCertificate=True"),
    ServiceLifetime.Scoped);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:53555", "https://localhost:7054")
               .AllowAnyMethod()
               .AllowAnyHeader()  // Allow all headers, including Content-Type
               .AllowCredentials(); // Allow sending cookies/authentication if needed
    });
});

var app = builder.Build();
//ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
ExcelPackage.License.SetNonCommercialOrganization("Local Government Unit");


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
    RequestPath = "/static" // Serve files under the /static path
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply the CORS policy correctly
app.UseCors("AllowBlazorOrigin"); // Apply the named CORS policy here

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
