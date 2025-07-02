# LogiTrack API Test Plan

## 1. Authentication & Authorization

- **Register User:**  
  - POST `/api/auth/register` with valid/invalid data.  
  - Expect: 201 Created or 400 Bad Request.

- **Login:**  
  - POST `/api/auth/login` with valid/invalid credentials.  
  - Expect: 200 OK with JWT or 401 Unauthorized.

- **Role Enforcement:**  
  - Try protected endpoints (e.g., DELETE inventory/order) as regular user and as Manager.  
  - Expect: 403 Forbidden for unauthorized roles.

## 2. Inventory Endpoints

- **Get Inventory (List):**  
  - GET `/api/inventory` (with/without pagination).  
  - Expect: 200 OK, correct data, timing and cache headers.

- **Add Inventory Item:**  
  - POST `/api/inventory` with valid/invalid data.  
  - Expect: 201 Created or 400 Bad Request.  
  - Check cache invalidation.

- **Delete Inventory Item:**  
  - DELETE `/api/inventory/{id}` as Manager and as regular user.  
  - Expect: 204 No Content (Manager), 403 Forbidden (User), 404 Not Found (invalid id).  
  - Check cache invalidation.

## 3. Order Endpoints

- **Get All Orders:**  
  - GET `/api/orders` (with/without pagination).  
  - Expect: 200 OK, correct data.

- **Get Order by ID:**  
  - GET `/api/orders/{id}` with valid/invalid id.  
  - Expect: 200 OK or 404 Not Found.

- **Create Order:**  
  - POST `/api/orders` with valid/invalid data.  
  - Expect: 201 Created or 400 Bad Request.  
  - Check inventory quantities update.

- **Delete Order:**  
  - DELETE `/api/orders/{id}` as Manager and as regular user.  
  - Expect: 204 No Content (Manager), 403 Forbidden (User), 404 Not Found (invalid id).  
  - Check inventory restock.

## 4. Error Handling

- Test all endpoints with missing/invalid data, unauthorized access, and non-existent resources.  
- Expect: Proper HTTP status codes and error messages.

## 5. Performance

- Measure response times for GET endpoints with and without cache.  
- Check `X-Elapsed-Milliseconds` and `X-Cache-Hit` headers.

---

**Tools:**  
- Use Swagger UI for manual testing.  
- Use Postman or curl for automated/scripted tests.  
- Optionally, write integration tests using xUnit and ASP.NET Core TestServer.
