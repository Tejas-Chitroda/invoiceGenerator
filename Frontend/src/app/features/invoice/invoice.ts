import { Component } from '@angular/core';
import { InvoiceService } from '../../core/services/invoice.service';
import { ProductService } from '../../core/services/product.service';
import { CustomerService } from '../../core/services/customer.service';
import { InvoiceModel } from '../../shared/models/invoice.model';
import { CustomerModel } from '../../shared/models/customer.model';
import { ProductModel } from '../../shared/models/product.model';
import { InvoiceDetail } from '../../shared/models/invoiceDetail.model';
import { InvoiceItemDto } from '../../shared/models/InvoiceItemDto.model';
import { InvoiceRequestDto } from '../../shared/models/InvoiceRequestDto.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InvoiceGenerate } from '../../shared/models/invoiceGenerate.model';

@Component({
  selector: 'app-invoice',
  imports: [CommonModule, FormsModule],
  templateUrl: './invoice.html',
  styleUrl: './invoice.css'
})
export class Invoice {



  productsMap: { [id: number]: string } = {};
  customers: CustomerModel[] = [];
  products: ProductModel[] = [];
  items: InvoiceItemDto[] = [];
  customerNames: { [invoiceId: number]: string } = {};
  selectedCustomerId: number = 0;
  selectedProductId: number = 0;
  quantity: number = 1;
  invoiceDetails: InvoiceDetail[] = [];
  invoices: InvoiceGenerate[] = [];
  selectedInvoiceDetails: InvoiceDetail[] = [];
  showModal: boolean = false;
  grandTotal: number = 0;


  constructor(private invoiceService: InvoiceService, private productService: ProductService, private customerService: CustomerService) { }

  invoiceData: InvoiceModel = {
    customerId: 0,
    productId: 0,
    quantity: 0
  };


  ngOnInit() {
    this.loadInvoices();
    this.loadProducts();
    this.loadCustomers();

  }

  loadInvoices() {
    this.invoiceService.getAllInvoices().subscribe((data: any) => {
      this.invoices = data;
      console.log(this.invoices);
    });
  }
  loadProducts() {
    this.productService.getAllProducts().subscribe((data: any[]) => {
      this.products = data;
    });
  }
  loadCustomers() {
    this.customerService.getAllCustomers().subscribe((data: any[]) => {
      this.customers = data;
    });
  }

  getCustomerName(customerId: number): string {
    const customer = this.customers.find(c => c.id === customerId);
    return customer ? customer.name : 'Unknown';
  }

  getProductName(productId: number): string {
    this.productService.getById(productId).subscribe((product: ProductModel) => {
      this.productsMap[productId] = product.name;
    });
    return this.productsMap[productId] || 'Unknown';
  }

  addItem() {
    const newItem: InvoiceItemDto = {
      productId: this.selectedProductId,
      quantity: this.quantity
    };
    this.items.push(newItem);
  }



  submitInvoice() {
    const invoiceData: InvoiceRequestDto = {
      customerId: this.selectedCustomerId,
      items: this.items
    };
    this.invoiceService.addInvoice(invoiceData).subscribe((response) => {
      console.log('Invoice created successfully:', response);
      this.loadInvoices();
      this.items = [];
      this.selectedCustomerId = 0;
      this.selectedProductId = 0;
      this.quantity = 1;
    });

    this.loadInvoices();
  }

  isFormValid(): boolean {
    return this.selectedCustomerId > 0 && this.selectedProductId > 0 && this.quantity > 0;
  }

  getInvoiceDetails(invoiceId: number): void {
    this.invoiceService.getInvoiceDetailByInvoiceId(invoiceId).subscribe((details: any) => {
      this.selectedInvoiceDetails = details as InvoiceDetail[];
      console.log('Invoice Details:', this.selectedInvoiceDetails);
      this.showModal = true;
    });
  }

  getInvoiceTotalByInvoiceId(invoiceId: number): number {
    const invoice = this.invoices.find(inv => inv.id === invoiceId);
    return invoice ? invoice.grandTotal : 0;
  }

  openInvoiceDetails(invoiceId: number) {
    this.getInvoiceDetails(invoiceId);
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
    this.selectedInvoiceDetails = [];
  }

}
