# InventoryController Endpoint Documentation

## 1. GetInventory
**Key:** GetInventory
```
Returns a list of all inventory items. Uses in-memory caching for 30 seconds to reduce database load.
- Anonymous access allowed.
- Cached result is refreshed every 30 seconds.
```

## 2. AddInventoryItem
**Key:** AddInventoryItem
```
Adds a new item to the inventory.
- item: The inventory item to add.
- Returns the created item with its assigned ID.
- Anonymous access allowed.
- Does NOT clear the cache (demo purpose).
```

## 3. DeleteInventoryItem
**Key:** DeleteInventoryItem
```
Removes an inventory item by ID. Only accessible to users with the Manager role.
- id: The ID of the inventory item to remove.
- Returns No content if successful, 404 if not found.
- Requires authentication and Manager role.
```

---

**Note:**
- Each block above corresponds to the XML docstring previously in the controller.
- You can reference these keys in your code comments for clarity.
- This approach keeps your code clean and documentation centralized.
