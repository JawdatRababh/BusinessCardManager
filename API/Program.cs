using Microsoft.OpenApi.Models;
using Service.Interfaces;
using Service.UnitOfWork;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using Domin.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Domain.Common;
using Domin.Models;
using Repository.Common;
using Repository.Interfaces;
using Repository.UnitOfWork;
using Serilog.Events;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region Ilogger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}") 
    .CreateLogger();


builder.Host.UseSerilog();
#endregion

#region Add Services to the container
builder.Services.AddScoped<IServiceUnitOfWork, ServiceUnitOfWork>();
builder.Services.AddHttpClient<ITPServiceUnitOfWork, TPServiceUnitOfWork>();


builder.Services.AddCors();
builder.Services.AddMvc().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();
#endregion

#region Configuration
var configuration = builder.Configuration;

#region Add the Db Context Options
SharedSettings.SqlConnectionString = configuration.GetSection("SharedSettings").GetValue<string>("BusinessCardSql");
builder.Services.AddDbContext<BusinessCardDBContext>(options =>
    options.UseSqlServer(SharedSettings.SqlConnectionString, b => b.MigrationsAssembly("API"))); 

#endregion

#region SharedSettings

#endregion

#endregion

#region Swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    option.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
    option.CustomSchemaIds(type => type.ToString());
});
#endregion

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.WriteIndented = true;
});

#region Application Build
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthorization();
//app.UseHttpsRedirection();
app.MapControllers();
app.Run();
#endregion