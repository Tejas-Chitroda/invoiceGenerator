import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { Observable } from "rxjs/internal/Observable";
import { CategoryModel } from "../../shared/models/category.model";

@Injectable({
    providedIn: 'root',
})
export class CategoryService {

    private apiUrl = '/api/category';

    constructor(private http: HttpClient) { }

    getAllCategories(): Observable<CategoryModel[]> {
        return this.http.get<CategoryModel[]>(this.apiUrl);
    }

    addCategory(category: CategoryModel): Observable<CategoryModel> {
        return this.http.post<CategoryModel>(this.apiUrl, {...category});
    }

    updateCategory(id: number, category: CategoryModel): Observable<CategoryModel> {
        return this.http.put<CategoryModel>(`${this.apiUrl}/${id}`, {...category});
    }

    getCategoryById(id: number): Observable<CategoryModel> {
        return this.http.get<CategoryModel>(`${this.apiUrl}/${id}`);
    }

    deleteCategory(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}