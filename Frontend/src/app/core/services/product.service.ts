import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ProductModel } from "../../shared/models/product.model";
import { ProductPrice } from "../../shared/models/productPrice.model";

@Injectable({
  providedIn: "root",
})
export class ProductService {
    private apiUrlProduct = '/api/product';
    private apiUrlProductPrice = 'api/productPrice';
  constructor(private http: HttpClient) {}

    getAllProducts() {
        return this.http.get<ProductModel[]>(this.apiUrlProduct);
    }

    addProduct(product: ProductModel) {
        return this.http.post<ProductModel>(this.apiUrlProduct, { ...product });
    }
    updateProduct(id: number, product: ProductModel) {
        return this.http.put<ProductModel>(`${this.apiUrlProduct}/${id}`, { ...product });
    }
    getById(id: number) {
        return this.http.get<ProductModel>(`${this.apiUrlProduct}/${id}`);
    }
    deleteProduct(id: number) {
        return this.http.delete<void>(`${this.apiUrlProduct}/${id}`);
    }

    getTodaysPrice(productId: number) {
        return this.http.get<number>(`${this.apiUrlProduct}/price/${productId}`);
    }

    addPrice(price : ProductPrice) {
        return this.http.post<ProductPrice>(`${this.apiUrlProductPrice}/AddPriceDefault`, { ...price });
    }
}