import { CustomerModel } from './customer.model';
import { InvoiceDetail } from './invoiceDetail.model';

export interface InvoiceModel {
  id?: number;
  customerId: number;
  productId : number; 
  quantity: number;
  invoiceDate?: Date; // optional, defaults to current date
  total?: number; // optional, calculated from invoice details
  customer?: CustomerModel;                // optional
  invoiceDetails?: InvoiceDetail[];   // optional list
}