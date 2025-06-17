import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, Subject, switchMap, tap } from 'rxjs';
import { ProductModel, ProductResponse } from '../../models/product.model';
import { ProductService } from '../../services/product.service';
import { RouterModule } from '@angular/router';

@Component({
    selector: 'app-products',
    imports: [CommonModule,FormsModule,RouterModule],
    templateUrl: './products.html',
    styleUrl: './products.css',
    standalone: true
})
export class Products implements OnInit{
    products:ProductModel[]=[];
    loading=false;
    loadingMore=false;
    search$= new Subject<string>();
    query='';
    limit=10;
    skip=0;
    hasMore=true;

    constructor(private productServive:ProductService){}

    ngOnInit(): void {
        this.handleSearch();
        this.loadProducts();
    }

    handleSearch(){
        this.search$.pipe(
            debounceTime(500),
            distinctUntilChanged(),
            tap((query)=>{
                this.query = query;
                this.skip = 0;
                this.hasMore = true;
                this.products = [];
                this.loading = true;
            }),
            switchMap((query) => this.productServive.getProducts(query, this.limit, this.skip))
        )
        .subscribe((res)=>{
            this.products = res.products;
            this.loading = false;
            this.skip = res.products.length;
            this.hasMore = res.products.length === this.limit;
        });
    }

    onSearchChange(event:Event){
        const input = event.target as HTMLInputElement;
        const query = input.value;
        this.search$.next(query);
    }

    loadProducts(){
        if(this.loadingMore || !this.hasMore) return;

        this.loadingMore = true;
        this.productServive.getProducts(this.query, this.skip, this.limit).subscribe((res)=>{
            this.products = [...this.products, ...res.products];
            this.skip += res.products.length;
            this.hasMore = res.products.length === this.limit;
            this.loadingMore = false;
        });
    }

    @HostListener('window:scroll',[])
    onScroll():void{
        const scrollPosition = window.innerHeight + window.scrollY;
        const pageHeight = document.documentElement.scrollHeight;
        if (scrollPosition >= pageHeight - 200 && !this.loading && !this.loadingMore) {
            this.loadProducts();
        }
    }
}
