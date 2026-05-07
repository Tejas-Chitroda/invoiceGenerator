import { Routes } from '@angular/router';
import { Customer } from './features/customer/customer';
import { Category } from './features/category/category';
import { Product } from './features/product/product';
import { Invoice } from './features/invoice/invoice';


export const routes: Routes = [

    {
        path: 'customer', component: Customer
    },
    {
        path: 'category', component: Category
    },
    {
        path: 'product', component: Product
    },
    {
        path: 'invoice', component: Invoice
    }
    
];
