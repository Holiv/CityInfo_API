//As an web application, it need to be hosted
//Initialize an instance of the WebApplicationBuilder 
using CityInfo.API;
using CityInfo.API.Models.Services;
using Microsoft.AspNetCore.StaticFiles;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
// Add services to the container.
// Builder is of type WebApplicationBuilder that has Service as a Property
// Service is of type IServiceCollection thar works as a Container to implement Dependency Injection
// IServiceCollection is provided with a lot of Extension Methods
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
  .AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddSingleton<CitiesDataStore>();

#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

// After services being added to the WebApplication through Dependency Injection it is build into a WebApplication
// The WebApplication type implements the IApplicationBuilder interface that provides the mechanisms to configure an application's request pipeline.
// It will define how the application will responde to individual HTTP requests
// The compononents that handle the requests are called middleware
var app = builder.Build();

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

