import { CategoryModel } from './category.model';
import { ProductPrice } from './productPrice.model';

export interface ProductModel {
  id?: number;
  name: string;
  description: string;
  taxPercentage: number;
  categoryId: number;
  category?: CategoryModel;           // optional, if you include Category info
  prices?: ProductPrice[];       // optional, list of prices
}