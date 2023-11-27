using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project1.Controllers;
using Swashbuckle.AspNetCore.SwaggerUI;
using Project1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddDbContext<ProjectDbContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("ProjectDbConnectionString")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


builder.Services.AddControllers();

builder.Services.AddHostedService<StockMonitoringService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Enable CORS before adding any middleware that might modify the response headers.
app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.EnableDeepLinking();
        c.DisplayRequestDuration();
        c.EnableValidator();
        c.SupportedSubmitMethods(SubmitMethod.Put, SubmitMethod.Post);
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
