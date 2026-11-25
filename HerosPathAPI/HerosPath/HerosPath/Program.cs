using HerosPath.DatabaseContext;
using HerosPath.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ContextDb>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TestDbString")),
    ServiceLifetime.Scoped);

builder.Services.AddScoped<HerosPathService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUnity",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowUnity");
app.UseAuthorization();
app.MapControllers();

app.Run();