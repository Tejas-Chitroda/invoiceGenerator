import { InvoiceItemDto } from "./InvoiceItemDto.model";

export interface InvoiceRequestDto {
  customerId: number;
  items: InvoiceItemDto[];
}