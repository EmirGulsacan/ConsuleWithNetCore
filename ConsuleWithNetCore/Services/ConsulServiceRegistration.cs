using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

public class ConsulServiceRegistration : IHostedService
{
    private readonly IConsulClient _consulClient; // Consul client
    private readonly IHostApplicationLifetime _lifetime; // Application lifetime management
    private readonly ILogger<ConsulServiceRegistration> _logger; // Logging
    private readonly IConfiguration _configuration; // Configuration settings
    private string _registrationId; // Service registration ID

    public ConsulServiceRegistration(IConsulClient consulClient, IHostApplicationLifetime lifetime, ILogger<ConsulServiceRegistration> logger, IConfiguration configuration)
    {
        _consulClient = consulClient;
        _lifetime = lifetime;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var serviceName = _configuration["Consul:ServiceName"];
        var servicePort = int.Parse(_configuration["Consul:ServicePort"]);
        var serviceAddress = _configuration["Consul:ServiceAddress"]; // Read address from configuration

        // Service registration information for Consul
        var registration = new AgentServiceRegistration
        {
            ID = _registrationId = Guid.NewGuid().ToString(),
            Name = serviceName,
            Address = serviceAddress, // Use address from configuration
            Port = servicePort,
            Checks = new[]
            {
                new AgentServiceCheck
                {
                    HTTP = $"http://{serviceAddress}:{servicePort}/health", // Health check URL
                    Interval = TimeSpan.FromSeconds(10) // Health check interval
                }
            }
        };

        _logger.LogInformation("Registering with Consul");
        await _consulClient.Agent.ServiceDeregister(registration.ID, cancellationToken); // Deregister existing service
        await _consulClient.Agent.ServiceRegister(registration, cancellationToken); // Register new service

        // Deregister service from Consul when application stops
        _lifetime.ApplicationStopping.Register(async () =>
        {
            _logger.LogInformation("Deregistering from Consul");
            await _consulClient.Agent.ServiceDeregister(_registrationId);
        });
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask; // Can be left empty
}
