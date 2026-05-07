import { Component } from '@angular/core';
import { ProductService } from '../../core/services/product.service';
import { CategoryService } from '../../core/services/category.service';
import { CategoryModel } from '../../shared/models/category.model';
import { ProductModel } from '../../shared/models/product.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductPrice } from '../../shared/models/productPrice.model';

@Component({
  selector: 'app-product',
  imports: [CommonModule, FormsModule],
  templateUrl: './product.html',
  styleUrl: './product.css'
})
export class Product {

  products: ProductModel[] = [];
  categories: CategoryModel[] = [];
  isEditing: boolean = false;
  editId: number | null = null;

  categoryNames: { [productId: number]: string } = {};

  productFormItem: ProductModel = {
    name: '',
    description: '',
    categoryId: 0,
    taxPercentage: 0, // Assuming taxPercentage can be null
  };

  priceFormItem: ProductPrice = {
    productId: 0,
    price: 0,
    effectiveFrom: '',
    effectiveTo: '',
    isDefault: false,
  };

  constructor(private productService: ProductService, private categoryService: CategoryService) { }


  ngOnInit() {
    this.loadProducts();
    this.loadCategories();
  }

  loadProducts() {
    this.productService.getAllProducts().subscribe((products) => {
      this.products = products;
      console.log(products);
    });
  }
  loadCategories() {
    this.categoryService.getAllCategories().subscribe((categories) => {
      this.categories = categories;
      console.log(categories);
    });
  }

  getCategoryName(category: any): string {
    // console.log(category.id);
    return category ? category.name : 'Unknown Category';
  }

  submitForm() {
    if (this.isEditing && this.editId) {
      const updatedProduct: ProductModel = { ...this.productFormItem };
      this.productService.updateProduct(this.editId, updatedProduct).subscribe(() => {
        this.resetForm();
        this.loadProducts();
      });
    } else {
      this.productService.addProduct(this.productFormItem).subscribe(() => {
        this.resetForm();
        this.loadProducts();
      });
    }
  }

  edit(product: ProductModel) {
    this.productFormItem = { name: product.name, description: product.description, taxPercentage: product.taxPercentage, categoryId: product.categoryId };
    this.editId = product.id!;
    this.isEditing = true;
  }


  resetForm() {
    this.productFormItem = { name: '', description: '', taxPercentage: 0, categoryId: 0 };
    this.isEditing = false;
    this.editId = null;
  }

  delete(id: number) {
    if (confirm('Are you sure?')) {
      this.productService.deleteProduct(id).subscribe(() => this.loadProducts());
    }
  }

  getTodayPrice(productId: number) {
    this.productService.getTodaysPrice(productId).subscribe((price) => {
      alert(`Today's price for product ID ${productId} is ${price}`);
    }, (error) => {
      console.error('Error fetching price:', error);
      alert('Failed to fetch price. Please try again later.');
    });
  }

  setPrice() {
    this.productService.addPrice(this.getDataFromPriceForm()).subscribe({
      next: (msg) => {
        alert(msg);
        this.priceFormItem = { productId: 0, price: 0, effectiveFrom: '', effectiveTo: '', isDefault: false };
      },
      error: (err) => {
        const errorMessage = typeof err.error === 'string'
          ? err.error
          : 'An error occurred while setting the price.';
        alert(errorMessage);
      }
    });
  }


  getDataFromPriceForm() {
    const form = { ...this.priceFormItem };

    const fromDate = form.effectiveFrom && form.effectiveFrom.trim() !== '';
    const toDate = form.effectiveTo && form.effectiveTo.trim() !== '';

    if (!fromDate || !toDate) {
      form.isDefault = true;
      form.effectiveFrom = undefined;
      form.effectiveTo = undefined;
    } else {
      form.isDefault = false;
    }

    return form;
  }

  onDefaultChange() {
    const val = this.priceFormItem.isDefault.toString().trim().toLowerCase();
    this.priceFormItem.isDefault = (val === 'yes');

    if (this.priceFormItem.isDefault) {
      this.priceFormItem.effectiveFrom = '';
      this.priceFormItem.effectiveTo = '';
    }
  }
}
