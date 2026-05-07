import { InvoiceModel } from './invoice.model';

export interface CustomerModel {
  id?: number;
  name: string;
  email?: string;        // optional (nullable in C#)
  invoices?: InvoiceModel[];  // optional navigation property
}