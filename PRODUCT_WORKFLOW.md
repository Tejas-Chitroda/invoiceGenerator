# Product Workflow

1. `GET /api/product` returns all products.
2. `GET /api/product/{id}` returns one product or `404`.
3. `POST /api/product` validates `ProductDto`, then creates the product.
4. `PUT /api/product/{id}` updates the product when the id is valid.
5. `DELETE /api/product/{id}` deletes the product or returns `404`.
6. `GET /api/product/price/{productId}` returns today's price or `404`.

The frontend uses `src/app/core/services/product.service.ts` to call these endpoints.