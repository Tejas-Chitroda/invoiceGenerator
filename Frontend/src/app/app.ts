import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Category } from "./features/category/category";

@Component({
  selector: 'app-root',
  standalone: true,    // ✅ IMPORTANT for standalone components
  imports: [
    RouterOutlet,
    MatButtonModule,
    HttpClientModule,
    RouterLink,
    RouterModule,
    CommonModule,
    Category
],
  templateUrl: './app.html',
  styleUrls: ['./app.css']   // ✅ use styleUrls (plural)
})
export class App {
  protected title = 'invoiceGenerator';
}
