import { ProductModel } from './product.model';

export interface ProductPrice {
  id?: number;
  productId: number;
  price: number;
  effectiveFrom?: string; // Can be a string or Date object
  isDefault: boolean;
  effectiveTo?: string;           // Optional because it can be null in C#
  product?: ProductModel;   
}