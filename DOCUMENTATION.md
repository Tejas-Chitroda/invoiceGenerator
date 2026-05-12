# InvoiceGenerator - Detailed Documentation

## Table of Contents
1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Setup Instructions](#setup-instructions)
4. [API Reference](#api-reference)
5. [Data Models](#data-models)
6. [Validation Rules](#validation-rules)
7. [Testing](#testing)

---

## Overview

InvoiceGenerator is a full-stack web application for managing invoices, products, customers, and categories. It uses ASP.NET Core (.NET 8) for the backend API and Angular 20 for the frontend.

**Key Features:**
- Customer management
- Product catalog with categories
- Dynamic product pricing
- Invoice generation with line items
- Tax calculation per product
- RESTful API with Swagger documentation

---

## Architecture

### Backend Stack
- **Framework:** ASP.NET Core 8.0 Web API
- **Database:** SQL Server LocalDB with Entity Framework Core 9.0
- **Pattern:** Repository + Unit of Work
- **Validation:** Service-layer validation with DTOs
- **API Documentation:** Swagger/OpenAPI

### Frontend Stack
- **Framework:** Angular 20
- **UI Library:** Angular Material
- **HTTP Client:** Angular HttpClient with proxy configuration
- **Routing:** Angular Router

### Project Structure

```
Backend/Invoice_Generator/
├── Controllers/         # API endpoints
├── Services/           
│   ├── Interfaces/     # Service contracts
│   └── Implementations/ # Business logic
├── Repository/         # Generic repository pattern
├── UoW/                # Unit of Work pattern
├── Data/               # DbContext
├── Model/              # Domain entities
├── DTOs/               # Data transfer objects
├── ModelConfigurations/ # EF Core configurations
└── Migrations/         # Database migrations

Frontend/src/app/
├── core/
│   └── services/       # API service layer
├── features/           # Feature modules (customer, product, invoice, category)
└── shared/
    └── models/         # TypeScript models
```

---

## Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+ and npm
- SQL Server LocalDB

### Backend Setup

1. Navigate to the backend directory:
   ```bash
   cd Backend/Invoice_Generator
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Update the connection string in `appsettings.json` if needed:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(LocalDb)\\MSSQLLocalDb;Database=InvoiceGenerator;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

4. Apply migrations to create the database:
   ```bash
   dotnet ef database update
   ```

5. Run the API:
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:5001` (or the port shown in console).  
Swagger UI: `https://localhost:5001/swagger`

### Frontend Setup

1. Navigate to the frontend directory:
   ```bash
   cd Frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm start
   ```

The app will be available at `http://localhost:4200`.

---

## API Reference

### Customer Endpoints

#### GET `/api/customer`
Get all customers.

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "John Doe",
    "email": "john@example.com"
  }
]
```

#### GET `/api/customer/{id}`
Get a customer by ID.

**Response:** `200 OK` or `404 Not Found`
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com"
}
```

#### POST `/api/customer`
Create a new customer.

**Request Body:**
```json
{
  "name": "John Doe",
  "email": "john@example.com"
}
```

**Validation:**
- `name`: Required, max 100 characters
- `email`: Optional, valid email format if provided

**Response:** `200 OK` or `400 Bad Request`

#### PUT `/api/customer/{id}`
Update an existing customer.

**Request Body:**
```json
{
  "name": "John Doe Updated",
  "email": "john.updated@example.com"
}
```

**Response:** `200 OK`, `400 Bad Request`, or `404 Not Found`

#### DELETE `/api/customer/{id}`
Delete a customer.

**Response:** `200 OK` or `404 Not Found`

---

### Category Endpoints

#### GET `/api/category`
Get all categories.

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "Electronics",
    "description": "Electronic products"
  }
]
```

#### GET `/api/category/{id}`
Get a category by ID.

**Response:** `200 OK` or `404 Not Found`

#### POST `/api/category`
Create a new category.

**Request Body:**
```json
{
  "name": "Electronics",
  "description": "Electronic products"
}
```

**Response:** `200 OK` or `400 Bad Request`

#### PUT `/api/category/{id}`
Update a category.

**Request Body:**
```json
{
  "name": "Updated Electronics",
  "description": "Updated description"
}
```

**Response:** `200 OK` or `400 Bad Request`

#### DELETE `/api/category/{id}`
Delete a category.

**Response:** `200 OK`

---

### Product Endpoints

#### GET `/api/product`
Get all products with category details.

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "Laptop",
    "description": "Gaming laptop",
    "taxPercentage": 18.0,
    "categoryId": 1,
    "categoryName": "Electronics"
  }
]
```

#### GET `/api/product/{id}`
Get a product by ID.

**Response:** `200 OK` or `404 Not Found`

#### POST `/api/product`
Create a new product.

**Request Body:**
```json
{
  "name": "Laptop",
  "description": "Gaming laptop",
  "taxPercentage": 18.0,
  "categoryId": 1
}
```

**Validation:**
- `name`: Required, max 100 characters
- `taxPercentage`: Required, between 0 and 100
- `categoryId`: Must reference an existing category

**Response:** `200 OK` or `400 Bad Request` with validation errors

#### PUT `/api/product/{id}`
Update a product.

**Request Body:** Same as POST

**Response:** `200 OK`, `400 Bad Request`, or `404 Not Found`

#### DELETE `/api/product/{id}`
Delete a product.

**Response:** `204 No Content` or `404 Not Found`

#### GET `/api/product/price/{productId}`
Get today's price for a product.

**Response:** `200 OK` or `404 Not Found`
```json
99.99
```

---

### Invoice Endpoints

#### GET `/api/invoice`
Get all invoices.

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "customerId": 1,
    "invoiceDate": "2026-05-11T00:00:00",
    "subTotal": 100.00,
    "taxTotal": 18.00,
    "grandTotal": 118.00
  }
]
```

#### GET `/api/invoice/{id}`
Get an invoice by ID.

**Response:** `200 OK` or `404 Not Found`

#### POST `/api/invoice`
Create a new invoice with line items.

**Request Body:**
```json
{
  "customerId": 1,
  "invoiceDate": "2026-05-11T00:00:00",
  "items": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 2,
      "quantity": 1
    }
  ]
}
```

**Process:**
1. Validates customer exists
2. Retrieves today's price for each product
3. Calculates subtotal, tax, and grand total per line item
4. Creates invoice and invoice details

**Response:** `200 OK` or `400 Bad Request`

#### DELETE `/api/invoice/{id}`
Delete an invoice and its details.

**Response:** `204 No Content` or `404 Not Found`

#### GET `/api/invoice/getInvoiceDetail/{invoiceId}`
Get invoice details (line items) for an invoice.

**Response:** `200 OK` or `400 Bad Request`
```json
[
  {
    "id": 1,
    "invoiceId": 1,
    "productId": 1,
    "quantity": 2,
    "rate": 99.99,
    "subTotal": 199.98,
    "tax": 35.99,
    "total": 235.97,
    "grandTotal": 235.97
  }
]
```

---

## Data Models

### Domain Entities

#### Customer
```csharp
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Email { get; set; }
    public ICollection<Invoice> Invoices { get; set; }
}
```

#### Category
```csharp
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Product> Products { get; set; }
}
```

#### Product
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal TaxPercentage { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<ProductPrice> Prices { get; set; }
}
```

#### ProductPrice
```csharp
public class ProductPrice
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public DateTime EffectiveDate { get; set; }
    public Product? Product { get; set; }
}
```

#### Invoice
```csharp
public class Invoice
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal GrandTotal { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<InvoiceDetail> InvoiceDetails { get; set; }
}
```

#### InvoiceDetail
```csharp
public class InvoiceDetail
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public decimal GrandTotal { get; set; }
    public Invoice? Invoice { get; set; }
    public Product? Product { get; set; }
}
```

### DTOs (Data Transfer Objects)

#### CustomerDto
```csharp
public class CustomerDto
{
    public string Name { get; set; }
    public string? Email { get; set; }
}
```

#### CategoryDto
```csharp
public class CategoryDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
}
```

#### ProductDto
```csharp
public class ProductDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal TaxPercentage { get; set; }
    public int CategoryId { get; set; }
}
```

#### InvoiceRequestDto
```csharp
public class InvoiceRequestDto
{
    public int CustomerId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public List<InvoiceItemDto> Items { get; set; }
}

public class InvoiceItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
```

---

## Validation Rules

### Customer Validation
- **Name**: Required, max length 100 characters
- **Email**: Optional, must be valid email format if provided

### Product Validation
- **Name**: Required, max length 100 characters
- **Description**: Optional
- **TaxPercentage**: Required, must be between 0 and 100
- **CategoryId**: Must reference an existing category

### Invoice Validation
- **CustomerId**: Must reference an existing customer
- **InvoiceDate**: Required
- **Items**: At least one item required
- **ProductId** (per item): Must reference an existing product with today's price available
- **Quantity** (per item): Must be greater than 0

### Validation Response Format
```json
{
  "isValid": false,
  "errors": {
    "Name": "Name is required",
    "CategoryId": "Category does not exist"
  }
}
```

---

## Testing

### Backend Tests

Test project: `Backend/InvoiceGenerator.Test`

**Test Coverage:**
- Controller unit tests with mocked services
- Service layer tests
- Validation tests

**Run tests:**
```bash
cd Backend/InvoiceGenerator.Test
dotnet test
```

**Example Test Structure:**
```csharp
[Fact]
public async Task AddProduct_ValidProduct_ReturnsOk()
{
    var productDto = new ProductDto { 
        Name = "P1", 
        CategoryId = 1, 
        TaxPercentage = 5 
    };
    _validationServiceMock
        .Setup(v => v.ValidateProductAsync(productDto))
        .ReturnsAsync(new ValidationResultDto { IsValid = true });

    var result = await _controller.AddProduct(productDto);

    var okResult = Assert.IsType<OkObjectResult>(result);
    Assert.Equal(productDto, okResult.Value);
}
```

### Frontend Tests

Run Angular tests:
```bash
cd Frontend
npm test
```

---

## Database Schema

### Relationships

- **Customer** 1:N **Invoice**
- **Category** 1:N **Product**
- **Product** 1:N **ProductPrice**
- **Product** 1:N **InvoiceDetail**
- **Invoice** 1:N **InvoiceDetail**

### Key Constraints

- All primary keys are auto-incremented integers
- Foreign keys enforce referential integrity
- `ProductPrice.EffectiveDate` allows time-based pricing
- `Product.TaxPercentage` is stored per product for flexibility

---

## Additional Notes

### CORS Configuration
The backend allows requests from `http://localhost:4200` in development mode.

### API Versioning
Not currently implemented. All endpoints are under `/api/[controller]`.

### Authentication/Authorization
Not currently implemented. All endpoints are publicly accessible.

### Error Handling
- `400 Bad Request`: Invalid input or validation failures
- `404 Not Found`: Resource not found
- `200 OK`: Successful GET/POST/PUT operations
- `204 No Content`: Successful DELETE operations

### Product Pricing Strategy
- Each product can have multiple prices with effective dates
- Invoice creation retrieves today's price for each product
- If no price exists for today, invoice creation fails for that item
