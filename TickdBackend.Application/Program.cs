using Microsoft.EntityFrameworkCore;
using TickdBackend.Application.Context;
using TickdBackend.Application.Helpers;
using TickdBackend.Application.Interfaces.Repositories;
using TickdBackend.Application.Interfaces.Services;
using TickdBackend.Application.Repositories;
using TickdBackend.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<ITickdService, TickdService>();
builder.Services.AddScoped<ITickdRepository, TickdRepository>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

builder.Services.AddDbContext<TickdDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PSQL"));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
