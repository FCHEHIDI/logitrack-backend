# LogiTrack API Test Outcomes (Swagger)

This file contains real test results and example responses from Swagger UI queries against the LogiTrack API.

---

## 1. Authentication & Authorization

### Register User
- **Request:**
  - POST `/api/auth/register`
  - Body: `{ "username": "testuser", "email": "test@example.com", "password": "Test123!" }`
- **Response:**
  - Status: 201 Created
  - Body: `{ "message": "Registration successful. Please check your email to confirm your account." }`

### Login
- **Request:**
  - POST `/api/auth/login`
  - Body: `{ "username": "testuser", "password": "Test123!" }`
- **Response:**
  - Status: 200 OK
  - Body: `{ "token": "<JWT_TOKEN>", "expires": "2025-07-02T15:00:00Z" }`

---

## 2. Inventory Endpoints

### Get Inventory (List)
- **Request:**
  - GET `/api/inventory?page=1&pageSize=2`
- **Response:**
  - Status: 200 OK
  - Headers: `X-Elapsed-Milliseconds: 12`, `X-Cache-Hit: False`
  - Body:
```json
[
  { "itemId": 1, "name": "Widget A", "quantity": 10, "location": "Warehouse 1" },
  { "itemId": 2, "name": "Widget B", "quantity": 5, "location": "Warehouse 2" }
]
```

### Add Inventory Item
- **Request:**
  - POST `/api/inventory`
  - Body: `{ "name": "Widget C", "quantity": 7, "location": "Warehouse 3" }`
- **Response:**
  - Status: 201 Created
  - Body: `{ "itemId": 3, "name": "Widget C", "quantity": 7, "location": "Warehouse 3" }`

### Delete Inventory Item (Manager)
- **Request:**
  - DELETE `/api/inventory/3` (with Manager JWT)
- **Response:**
  - Status: 204 No Content

---

## 3. Order Endpoints

### Create Order
- **Request:**
  - POST `/api/orders`
  - Body:
```json
{
  "sessionId": "abc123-session-token",
  "customerName": "Jane Doe",
  "datePlaced": "2025-07-02T14:30:00Z",
  "items": [
    { "name": "Widget A", "quantity": 2, "location": "Warehouse 1" },
    { "name": "Widget B", "quantity": 1, "location": "Warehouse 2" }
  ]
}
```
- **Response:**
  - Status: 201 Created
  - Body: `{ "orderId": 1, "customerName": "Jane Doe", "datePlaced": "2025-07-02T14:30:00Z", "items": [ ... ] }`

### Get All Orders
- **Request:**
  - GET `/api/orders?page=1&pageSize=2`
- **Response:**
  - Status: 200 OK
  - Body:
```json
[
  { "orderId": 1, "customerName": "Jane Doe", "datePlaced": "2025-07-02T14:30:00Z", "items": [ ... ] }
]
```

### Delete Order (Manager)
- **Request:**
  - DELETE `/api/orders/1` (with Manager JWT)
- **Response:**
  - Status: 204 No Content

---

## 4. Error Handling Examples

### Unauthorized Access
- **Request:**
  - DELETE `/api/inventory/1` (as regular user)
- **Response:**
  - Status: 403 Forbidden
  - Body: `{ "error": "Forbidden" }`

### Not Found
- **Request:**
  - GET `/api/orders/9999`
- **Response:**
  - Status: 404 Not Found
  - Body: `{ "error": "Order not found" }`

---

These results were obtained using Swagger UI and represent real API behavior as of July 2, 2025. For more details, see the full API documentation and test plan.
