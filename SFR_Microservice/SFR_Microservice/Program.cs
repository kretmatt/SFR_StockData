using Microsoft.EntityFrameworkCore;
using MS;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add services to the container.
builder.Services.AddDbContext<StocksContext>(options =>
{
    options.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionString")?.Trim('\"')??throw new Exception("There is no connection string for the database"));
});

builder.Services.AddHostedService<KafkaConsumerHandler>();
builder.Services.AddControllers();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();