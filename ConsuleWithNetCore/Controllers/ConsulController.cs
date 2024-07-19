using Consul;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ConsuleWithNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsulController : ControllerBase
    {
        private readonly IConsulClient _consulClient;

        public ConsulController(IConsulClient consulClient)
        {
            _consulClient = consulClient;
        }

        // Endpoint to get the value of a key-value pair
        [HttpGet("kv/{key}")]
        public async Task<IActionResult> GetValue(string key)
        {
            var result = await _consulClient.KV.Get(key);
            if (result.Response == null)
            {
                return NotFound();
            }

            var value = System.Text.Encoding.UTF8.GetString(result.Response.Value);
            return Ok(value);
        }

        // Endpoint to set the value of a key-value pair
        [HttpPut("kv/{key}")]
        public async Task<IActionResult> SetValue(string key, [FromBody] string value)
        {
            var kvPair = new KVPair(key)
            {
                Value = System.Text.Encoding.UTF8.GetBytes(value)
            };

            var result = await _consulClient.KV.Put(kvPair);
            return result.Response ? Ok() : StatusCode(500);
        }

        // Endpoint to list all services
        [HttpGet("services")]
        public async Task<IActionResult> GetServices()
        {
            var services = await _consulClient.Agent.Services();
            return Ok(services.Response.Values);
        }

        // Endpoint to acquire a lock
        [HttpPost("locks/acquire/{key}")]
        public async Task<IActionResult> AcquireLock(string key)
        {
            var lockId = Guid.NewGuid().ToString();
            var lockPair = new KVPair(key)
            {
                Value = System.Text.Encoding.UTF8.GetBytes(lockId),
                Session = lockId
            };

            var result = await _consulClient.KV.Put(lockPair);
            return result.Response ? Ok(lockId) : StatusCode(500, "Failed to acquire lock.");
        }

        // Endpoint to release a lock
        [HttpDelete("locks/release/{key}")]
        public async Task<IActionResult> ReleaseLock(string key)
        {
            var result = await _consulClient.KV.Delete(key);
            return result.Response ? Ok() : StatusCode(500, "Failed to release lock.");
        }

        // Endpoint to get the value of a lock
        [HttpGet("locks/value/{key}")]
        public async Task<IActionResult> GetLockValue(string key)
        {
            var result = await _consulClient.KV.Get(key);
            if (result.Response != null)
            {
                var lockValue = System.Text.Encoding.UTF8.GetString(result.Response.Value);
                return Ok(new { LockValue = lockValue });
            }

            return NotFound("Lock not found.");
        }

        // Endpoint to check if a lock exists
        [HttpHead("locks/exists/{key}")]
        public async Task<IActionResult> CheckLockExists(string key)
        {
            var result = await _consulClient.KV.Get(key);
            return result.Response != null ? Ok() : NotFound("Lock not found.");
        }

       
    }
}
