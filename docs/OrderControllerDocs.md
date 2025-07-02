# OrderController Endpoint Documentation

## 1. GetAllOrders
**Key:** GetAllOrders
```
Returns all orders with their items. Uses eager loading and no tracking for performance.
```

## 2. GetOrderById
**Key:** GetOrderById
```
Returns a single order by ID, including its items. Uses eager loading and no tracking.
- id: Order ID
```

## 3. CreateOrder
**Key:** CreateOrder
```
Creates a new order and updates inventory quantities accordingly.
- order: Order to create
```

## 4. DeleteOrder
**Key:** DeleteOrder
```
Deletes an order by ID and restocks inventory. Only accessible to users with the Manager role.
- id: Order ID
```

---

**Note:**
- Each block above corresponds to the XML docstring previously in the controller.
- You can reference these keys in your code comments for clarity.
- This approach keeps your code clean and documentation centralized.
