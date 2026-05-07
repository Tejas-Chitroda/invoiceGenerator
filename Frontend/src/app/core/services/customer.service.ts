import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CustomerModel } from "../../shared/models/customer.model";
import { Observable } from "rxjs/internal/Observable";

@Injectable({
    providedIn: 'root',
})
export class CustomerService {

    private apiUrl = '/api/customer';

    constructor(private http: HttpClient) { }

    getAllCustomers(): Observable<CustomerModel[]> {
        return this.http.get<CustomerModel[]>(this.apiUrl);
    }

    addCustomer(customer: CustomerModel): Observable<CustomerModel> {
        return this.http.post<CustomerModel>(this.apiUrl, {...customer});
    }

    updateCustomer(id: number, customer: CustomerModel): Observable<CustomerModel> {
        return this.http.put<CustomerModel>(`${this.apiUrl}/${id}`, {...customer});
    }

    getById(id: number): Observable<CustomerModel> {
        return this.http.get<CustomerModel>(`${this.apiUrl}/${id}`);
    }

    deleteCustomer(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}