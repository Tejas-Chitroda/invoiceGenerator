import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CategoryService } from '../../core/services/category.service';
import { CategoryModel } from '../../shared/models/category.model';

@Component({
  selector: 'app-category',
  imports: [FormsModule , CommonModule],
  templateUrl: './category.html',
  styleUrl: './category.css'
})
export class Category {

  categories: CategoryModel[] = [];
  category: CategoryModel = {
    name: '',
    description: ''
  };
  isEdit: boolean = false;

 constructor(private categoryService: CategoryService) { }

 ngOnInit() {
   this.getAllCategories(); 
 }

 getAllCategories()
 {
   this.categoryService.getAllCategories().subscribe(
     (data) => {
       this.categories = data;
     },
     (error) => {
       console.error('Error fetching categories:', error);
     }
   );
 }

 submitCategory()
 {
  if (this.isEdit) {
    this.categoryService.updateCategory(this.category.id!, this.category).subscribe(
      (data) => {
        this.getAllCategories();
        this.resetForm();
      },
      (error) => {
        console.error('Error updating category:', error);
      }
    );
  } else {
    this.categoryService.addCategory(this.category).subscribe(
      (data) => {
        this.getAllCategories();
        this.resetForm();
      },
      (error) => {
        console.error('Error adding category:', error);
      }
    );
  }
 }

 editForm(category: CategoryModel)
 {
  this.category = { ...category };
  this.isEdit = true;
 }
  deleteCategoryById(id: number)
  {
    this.categoryService.deleteCategory(id).subscribe(
      () => {
        this.getAllCategories();
      },
      (error) => {
        console.error('Error deleting category:', error);
      }
    );
  }

 resetForm()
 {
  this.category.description = '';
  this.category.name = '';
  this.isEdit = false;
 }

}
