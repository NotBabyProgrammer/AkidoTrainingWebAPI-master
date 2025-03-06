using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AkidoTrainingWebAPI.DataAccess.Data;
using AkidoTrainingWebAPI.BusinessLogic.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AkidoTrainingWebAPIContext");
builder.Services.AddDbContext<AkidoTrainingWebAPIContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowSpecificOrigin",
    build =>
    {
        build.WithOrigins("*")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 104857600; // 100 MB in bytes
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 104857600; // 100 MB in bytes
});

builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<AreasRepository>();
builder.Services.AddScoped<DistrictsRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
 app.UseSwagger();
 app.UseSwaggerUI();
}
app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
