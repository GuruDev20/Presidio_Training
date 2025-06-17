import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ProductModel } from '../../models/product.model';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ProductService } from '../../services/product.service';

@Component({
    selector: 'app-product-detail',
    imports: [CommonModule,RouterModule],
    templateUrl: './product-detail.html',
    styleUrl: './product-detail.css',
    standalone: true
})
export class ProductDetail implements OnInit {
    products:ProductModel|null=null;
    isLoading=true;
    error:string|null=null;

    constructor(private route:ActivatedRoute,private router:Router,private productService:ProductService){}

    ngOnInit(): void {
        const id=Number(this.route.snapshot.paramMap.get('id'));
        if(isNaN(id) || id<=0){
            this.error='Invalid product ID';
            this.isLoading=false;
            return;
        }

        this.productService.getproductsById(id).subscribe({
            next:(data)=>{
                this.products=data;
                this.isLoading=false;
            },
            error:(err)=>{
                this.error='Error fetching product details';
                this.isLoading=false;
            }
        });
    }

    goBack(){
        this.router.navigate(['/products']);
    }
}
