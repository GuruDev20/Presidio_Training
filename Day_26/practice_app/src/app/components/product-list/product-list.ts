import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

@Component({
    selector: 'app-product-list',
    templateUrl: './product-list.html',
    imports: [CommonModule],
    styleUrls: ['./product-list.css'],
})
export class ProductListComponent {
    cartCount = 0;

    products = [
        { 
            id: 1, 
            name: 'Laptop', 
            image: 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?q=80&w=3726&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D' 
        },
        { 
            id: 2, 
            name: 'Mobile', 
            image: 'https://images.unsplash.com/photo-1591337676887-a217a6970a8a?q=80&w=3880&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D' 
        },
        { 
            id: 3, 
            name: 'Airpods', 
            image: 'https://images.unsplash.com/photo-1606841837239-c5a1a4a07af7?q=80&w=3774&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D' 
        }
    ];

    addToCart() {
        this.cartCount++;
    }
}