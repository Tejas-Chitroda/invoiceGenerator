# Project Flow

1. The Angular app starts in `Frontend/src/app/app.ts` and routes to `customer`, `category`, `product`, and `invoice` pages.
2. Each feature page calls its service in `Frontend/src/app/core/services`.
3. Services send HTTP requests to the ASP.NET Core API under `/api/*`.
4. API controllers validate input, call application services, and persist data through EF Core and SQL Server LocalDB.
5. In development, Swagger is available and CORS allows requests from `http://localhost:4200`.

## Main Areas

- `Customer`, `Category`, and `Product` handle CRUD flows.
- `Product` also exposes today's price.
- `Invoice` creates and reads invoices and invoice details.