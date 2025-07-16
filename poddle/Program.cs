using poddle.Models;
using poddle.Repositories;
using poddle.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure EposNow settings
builder.Services.Configure<EposNowConfig>(
    builder.Configuration.GetSection("EposNow"));

// Register HTTP client
builder.Services.AddHttpClient<IEposNowRepository, EposNowRepository>();

// Register services
builder.Services.AddScoped<IEposNowRepository, EposNowRepository>();
builder.Services.AddScoped<IEposNowService, EposNowService>();

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
