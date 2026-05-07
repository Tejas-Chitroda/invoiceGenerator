import { ProductModel } from './product.model';

export interface CategoryModel {
  id?: number;
  name: string;
  description?: string;    // Optional field
  products?: ProductModel[];    // Optional - only if you want nested products when loading a category
}