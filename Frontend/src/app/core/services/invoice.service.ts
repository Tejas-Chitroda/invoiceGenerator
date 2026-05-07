import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class InvoiceService {

    private apiUrl = '/api/invoice';

    constructor(private http: HttpClient) { }

    getAllInvoices() {
        return this.http.get(this.apiUrl);
    }
    addInvoice(invoice: any) {
        return this.http.post(this.apiUrl, invoice);
    }
    getInvoiceById(id: number) {
        return this.http.get(`${this.apiUrl}/${id}`);
    }
    getInvoiceDetailsById(id: number) {
        return this.http.get(`${this.apiUrl}/${id}/details`);
    }
    getInvoiceDetailByInvoiceId(invoiceId: number) {
        return this.http.get(`${this.apiUrl}/getInvoiceDetail/${invoiceId}`);
    }
}