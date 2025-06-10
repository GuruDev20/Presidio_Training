import { Component } from '@angular/core';
import { ProductListComponent } from "./components/product-list/product-list";
import { CustomerDetailsComponent } from "./components/customer-details/customer-details";

@Component({
    selector: 'app-root',
    imports: [CustomerDetailsComponent, ProductListComponent],
    templateUrl: './app.html',
    styleUrl: './app.css'
})
export class App {
    protected title = 'practice_app';
}
