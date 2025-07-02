using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LogiTrack.Models;
using LogiTrack.Data;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly LogiTrackContext _context;
        private readonly ILogger<OrderController> _logger;


        public OrderController(LogiTrackContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Helper to fetch inventory items by name/location
        private async Task<List<InventoryItem>> FetchInventoryItemsAsync(IEnumerable<InventoryItem> items)
        {
            if (items == null) return new List<InventoryItem>();
            var itemNames = items.Select(i => i.Name).ToList();
            var itemLocations = items.Select(i => i.Location).ToList();
            return await _context.InventoryItems
                .Where(ii => itemNames.Contains(ii.Name) && itemLocations.Contains(ii.Location))
                .ToListAsync();
        }



        // GET: api/orders
        [AllowAnonymous]
        [HttpGet]
        // [Doc: GetAllOrders]
        public async Task<IActionResult> GetAllOrders()
        {
            // This method handles GET requests to /api/orders
            // - Allows anonymous access (no authentication required)
            // - Uses eager loading (.Include) to fetch related order items in a single query
            // - Uses .AsNoTracking() for read-only performance (no change tracking)
            // - Returns a list of all orders with their items
            // Pagination parameters
            int page = 1, pageSize = 50;
            if (Request.Query.ContainsKey("page")) int.TryParse(Request.Query["page"], out page);
            if (Request.Query.ContainsKey("pageSize")) int.TryParse(Request.Query["pageSize"], out pageSize);
            _logger.LogInformation("[GetAllOrders] Page: {Page}, PageSize: {PageSize}", page, pageSize);
            var orders = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Ok(orders);
        }

        // GET: api/orders/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        // [Doc: GetOrderById]
        public async Task<IActionResult> GetOrderById(int id)
        {
            _logger.LogInformation("[GetOrderById] Requested OrderId: {OrderId}", id);
            var order = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

        // POST: api/orders
        [AllowAnonymous]
        [HttpPost]
        // [Doc: CreateOrder]

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            // This method handles POST requests to /api/orders
            // - Allows anonymous access
            // - Adds a new order to the database
            // - Updates inventory quantities for each ordered item
            // - Uses a dictionary for efficient inventory lookup
            // - Returns the created order with its ID

            _logger.LogInformation("[CreateOrder] Creating order for customer: {CustomerName}, SessionId: {SessionId}, Items: {ItemCount}", order.CustomerName, order.SessionId, order.Items?.Count ?? 0);
            await _context.Orders.AddAsync(order);
            var inventoryItems = await FetchInventoryItemsAsync(order.Items ?? Enumerable.Empty<InventoryItem>());
            foreach (var orderedItem in order.Items ?? Enumerable.Empty<InventoryItem>())
            {
                _logger.LogInformation("[CreateOrder] Processing item: {Name} @ {Location}, Qty: {Qty}", orderedItem.Name, orderedItem.Location, orderedItem.Quantity);
                var inventoryItem = inventoryItems.FirstOrDefault(ii => ii.Name == orderedItem.Name && ii.Location == orderedItem.Location);
                if (inventoryItem != null)
                {
                    inventoryItem.Quantity -= orderedItem.Quantity;
                    _logger.LogInformation("[CreateOrder] Updated inventory: {Name} @ {Location}, New Qty: {Qty}", inventoryItem.Name, inventoryItem.Location, inventoryItem.Quantity);
                }
            }
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        // DELETE: api/orders/{id}
        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        // [Doc: DeleteOrder]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            // This method handles DELETE requests to /api/orders/{id}
            // - Requires authentication and Manager role
            // - Finds the order and its items
            // - Restocks inventory for each item in the order
            // - Removes the order from the database
            // - Returns 204 No Content if successful, 404 if not found

            _logger.LogInformation("[DeleteOrder] Deleting order with OrderId: {OrderId}", id);
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
                return NotFound();
            var inventoryItems = await FetchInventoryItemsAsync(order.Items ?? Enumerable.Empty<InventoryItem>());
            foreach (var orderedItem in order.Items ?? Enumerable.Empty<InventoryItem>())
            {
                _logger.LogInformation("[DeleteOrder] Restocking item: {Name} @ {Location}, Qty: {Qty}", orderedItem.Name, orderedItem.Location, orderedItem.Quantity);
                var inventoryItem = inventoryItems.FirstOrDefault(ii => ii.Name == orderedItem.Name && ii.Location == orderedItem.Location);
                if (inventoryItem != null)
                {
                    inventoryItem.Quantity += orderedItem.Quantity;
                    _logger.LogInformation("[DeleteOrder] Updated inventory: {Name} @ {Location}, New Qty: {Qty}", inventoryItem.Name, inventoryItem.Location, inventoryItem.Quantity);
                }
            }
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}