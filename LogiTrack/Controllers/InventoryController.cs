using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LogiTrack.Models;
using LogiTrack.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

//Get a single inventory item by ID
// (Moved method to correct place below)

namespace LogiTrack.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize] // Require authentication for all endpoints by default
	public class InventoryController : ControllerBase
	{
		private readonly IMemoryCache _cache;
		private readonly LogiTrackContext _context;
		private readonly ILogger<InventoryController> _logger;
		public InventoryController(LogiTrackContext context, IMemoryCache cache, ILogger<InventoryController> logger)
		{
			_context = context;
			_cache = cache;
			_logger = logger;
		}

		// [Doc: GetInventory]
		[AllowAnonymous]
		[HttpGet("inventory")]
		public async Task<IActionResult> GetInventory()
		{
			int page = 1, pageSize = 50;
			if (Request.Query.ContainsKey("page")) int.TryParse(Request.Query["page"], out page);
			if (Request.Query.ContainsKey("pageSize")) int.TryParse(Request.Query["pageSize"], out pageSize);

			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			string cacheKey = $"inventory_list_{page}_{pageSize}";
			bool cacheHit = _cache.TryGetValue(cacheKey, out List<InventoryItem>? items);
			if (!cacheHit)
			{
				items = await _context.InventoryItems.AsNoTracking()
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ToListAsync();
				var cacheEntryOptions = new MemoryCacheEntryOptions()
					.SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
				_cache.Set(cacheKey, items, cacheEntryOptions);
				_logger.LogInformation("[GetInventory] Cache miss. Data loaded from DB. Page: {Page}, PageSize: {PageSize}", page, pageSize);
			}
			else
			{
				_logger.LogInformation("[GetInventory] Cache hit. Page: {Page}, PageSize: {PageSize}", page, pageSize);
			}
			stopwatch.Stop();
			Response.Headers["X-Elapsed-Milliseconds"] = stopwatch.ElapsedMilliseconds.ToString();
			Response.Headers["X-Cache-Hit"] = cacheHit.ToString();
			return Ok(items);
		}


		// [Doc: AddInventoryItem]
		[AllowAnonymous]
		[HttpPost("inventory")]
		public async Task<IActionResult> AddInventoryItem([FromBody] InventoryItem item)
		{
			_logger.LogInformation("[AddInventoryItem] Adding new inventory item: {Name} @ {Location}", item.Name, item.Location);
			await _context.InventoryItems.AddAsync(item);
			await _context.SaveChangesAsync();
			InvalidateInventoryCache();
			_logger.LogInformation("[AddInventoryItem] Item added and cache invalidated.");
			return CreatedAtAction(nameof(GetInventory), new { id = item.ItemId }, item);
		}

		// [Doc: DeleteInventoryItem]
		[Authorize(Roles = "Manager")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteInventoryItem(int id)
		{
			_logger.LogInformation("[DeleteInventoryItem] Attempting to delete inventory item with ID: {Id}", id);
			var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.ItemId == id);
			if (item == null)
			{
				_logger.LogWarning("[DeleteInventoryItem] Item not found: {Id}", id);
				return NotFound();
			}
			_context.InventoryItems.Remove(item);
			await _context.SaveChangesAsync();
			InvalidateInventoryCache();
			_logger.LogInformation("[DeleteInventoryItem] Item deleted and cache invalidated: {Id}", id);
			return NoContent();
		}
		// Helper to invalidate all paged inventory cache keys
		private void InvalidateInventoryCache()
		{
			// NOTE: MemoryCache does not support enumeration of all keys in a safe way.
			// In production, use a distributed cache or a custom cache manager for pattern-based eviction.
			// Here, we simulate by removing the first few paged cache keys.
			for (int page = 1; page <= 5; page++)
			{
				for (int pageSize = 10; pageSize <= 100; pageSize += 10)
				{
					string key = $"inventory_list_{page}_{pageSize}";
					_cache.Remove(key);
				}
			}
		}
	}
}