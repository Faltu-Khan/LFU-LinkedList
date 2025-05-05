using Microsoft.AspNetCore.Mvc;
using LFUCache.Library.Interfaces;
using LFUCache.Library.Exceptions;
using LFUCache.Library.Models;
using System.Linq;

namespace LFUCache.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly ICache<int, string> _cache;
        private static int _keyGenerator = 100; // Starting key value

        public CacheController(ICache<int, string> cache)
        {
            _cache = cache;
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] string value)
        {
            try
            {
                int newKey = ++_keyGenerator;
                _cache.Put(newKey, value);
                return Ok(new { Message = "Value added.", Key = newKey });
            }
            catch (DuplicateItemException e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpGet("get/{key}")]
        public IActionResult Get(int key)
        {
            try
            {
                return Ok(_cache.Get(key));
            }
            catch (ItemNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPut("update/{key}")]
        public IActionResult Update(int key, [FromBody] string value)
        {
            try
            {
                _cache.Update(key, value);
                return Ok("Value updated.");
            }
            catch (ItemNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpDelete("remove/{key}")]
        public IActionResult Remove(int key)
        {
            try
            {
                _cache.Remove(key);
                return Ok("Key removed.");
            }
            catch (ItemNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var items = _cache.GetAll()
                              .Select(i => new CacheItemDto {
                                  Key = i.Key,
                                  Value = i.Value,
                                  Frequency = i.Frequency,
                                  LastAccess = i.LastAccess
                              });
            return Ok(items);
        }
    }
}