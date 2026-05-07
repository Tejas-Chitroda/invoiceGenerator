import { InvoiceModel } from './invoice.model';
import { ProductModel } from './product.model';

export interface InvoiceDetail {
  id: number;
  invoiceId: number;
  productId: number;
  quantity: number;
  rate: number;
  subTotal: number;
  tax: number;
  total: number;
  grandTotal: number;
  invoice?: InvoiceModel;     // optional navigation
  product?: ProductModel;     // optional navigation
}