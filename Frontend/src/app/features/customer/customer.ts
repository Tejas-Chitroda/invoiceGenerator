import { Component } from '@angular/core';
import { CustomerService } from '../../core/services/customer.service';
import { CustomerModel } from '../../shared/models/customer.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-customer',
  imports: [CommonModule , FormsModule],
  templateUrl: './customer.html',
  styleUrl: './customer.css'
})
export class Customer {

  customer : CustomerModel = {
    name: '',
    email: '',
  }

  customerData: CustomerModel[] = [];
  isEdit :boolean = false;

  constructor(private customerService: CustomerService) {
    
  }
  ngOnInit() {
    console.log('Customer component initialized');
    this.getCustomers();
  }
  getCustomers()
  {
    this.customerService.getAllCustomers().subscribe(customers => {
      this.customerData = customers;
    });
  }

  submitCustomer() {
    if (this.isEdit) {
      this.customerService.updateCustomer(this.customer.id!, this.customer).subscribe(updatedCustomer => {
        console.log('Customer updated:', updatedCustomer);
        this.resetForm();
        this.getCustomers();
      }, error => {
        console.error('Error updating customer:', error);
      });
    } else {
      this.customerService.addCustomer(this.customer).subscribe(newCustomer => {
        console.log('Customer added:', newCustomer);
        this.resetForm();
      }, error => {
        console.error('Error adding customer:', error);
      });
      this.getCustomers();
    }
    this.getCustomers();
  }

  resetForm()
  {
    this.customer = { name: '', email: '' };
    this.isEdit = false; // Reset to add mode
  }
  
  getById(id: number) {
    this.customerService.getById(id).subscribe(customer => {
      this.customer = customer;
      this.isEdit = true; // Set to edit mode
    }, error => {
      console.error('Error fetching customer by ID:', error);
    });
  }

  deleteCustomerById(id: number) {
    this.customerService.deleteCustomer(id).subscribe(() => {
      console.log('Customer deleted');
      this.getCustomers(); // Refresh the customer list
    }, error => {
      console.error('Error deleting customer:', error);
    });
  }

  editForm(customer: CustomerModel) {
    this.customer = { ...customer }; // Spread operator to copy properties
    this.isEdit = true; // Set to edit mode
  }
}
