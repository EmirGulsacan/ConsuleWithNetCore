using Consul;
using Winton.Extensions.Configuration.Consul;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Consul client
builder.Services.AddSingleton<IConsulClient>(sp =>
{
    var consulAddress = builder.Configuration["Consul:Address"];
    return new ConsulClient(config => config.Address = new Uri(consulAddress));
});

// Add hosted service for Consul service registration
builder.Services.AddHostedService<ConsulServiceRegistration>();

// Add distributed lock provider
builder.Services.AddSingleton<IDistributedLockProvider, ConsulDistributedLockProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
