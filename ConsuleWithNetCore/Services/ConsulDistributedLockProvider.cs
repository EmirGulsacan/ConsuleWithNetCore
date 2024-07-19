using Consul;
using Microsoft.Extensions.Configuration;

public interface IDistributedLockProvider
{
    Task<string> AcquireLockAsync(string key); 
    Task ReleaseLockAsync(string key);
    Task<string> GetLockAsync(string key);
    Task<bool> LockExistsAsync(string key);
}

public class ConsulDistributedLockProvider : IDistributedLockProvider
{
    private readonly IConsulClient _consulClient; 
    private readonly IConfiguration _configuration; 

    public ConsulDistributedLockProvider(IConsulClient consulClient, IConfiguration configuration)
    {
        _consulClient = consulClient;
        _configuration = configuration;
    }

    // Method to acquire a lock
    public async Task<string> AcquireLockAsync(string key)
    {
        var lockId = Guid.NewGuid().ToString(); 
        var lockPair = new KVPair(key)
        {
            Value = System.Text.Encoding.UTF8.GetBytes(lockId), 
            Session = lockId 
        };

        var result = await _consulClient.KV.Put(lockPair); 
        return result.Response ? lockId : null; 
    }

    // Method to release a lock
    public async Task ReleaseLockAsync(string key)
    {
        await _consulClient.KV.Delete(key); 
    }

    // Method to get the value of a lock
    public async Task<string> GetLockAsync(string key)
    {
        var result = await _consulClient.KV.Get(key); 
        return result.Response != null ? System.Text.Encoding.UTF8.GetString(result.Response.Value) : null; 
    }

    // Method to check if a lock exists
    public async Task<bool> LockExistsAsync(string key)
    {
        var result = await _consulClient.KV.Get(key); 
        return result.Response != null;
    }
}
