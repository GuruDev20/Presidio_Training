import { Component, HostListener, OnInit } from '@angular/core';
import { Product } from '../../models/product';
import { debounceTime, distinctUntilChanged, Subject, switchMap, tap } from 'rxjs';
import { ProductService } from '../../services/product.service';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-home',
    imports: [CommonModule],
    templateUrl: './home.html',
    styleUrl: './home.css',
    standalone: true
})
export class Home implements OnInit{
    products:Product[]=[];
    searchTerm:string='';
    searchSubject=new Subject<string>();
    skip:number=0;
    isLoading=false;
    allLoaded=false;

    constructor(private productService:ProductService) {}

    ngOnInit(): void {
        this.searchSubject.pipe(
            debounceTime(400),
            distinctUntilChanged(),
            tap(()=>{
                this.products = [];
                this.skip = 0;
                this.allLoaded = false;
                this.isLoading = true;
            }),
            switchMap(term=>{
                this.searchTerm = term;
                return this.productService.searchProducts(term, this.skip);
            })
        ).subscribe(data=>{
            this.products=data.products;
            this.isLoading = false;
            this.skip += 10;
        });
        this.searchSubject.next('');
    }

    onSearch(event:Event): void {
        const input = event.target as HTMLInputElement;
        const value = input.value;
        this.searchSubject.next(value);
    }

    @HostListener('window:scroll',[])
    onScroll():void{
        if((window.innerHeight+window.scrollY)>=document.body.offsetHeight-200 && !this.isLoading && !this.allLoaded){
            this.loadMore();
        }
    }

    loadMore(){
        this.isLoading = true;
        this.productService.searchProducts(this.searchTerm,this.skip).subscribe(data=>{
            if(data.products.length===0){
                this.allLoaded = true;
            }
            this.products = [...this.products, ...data.products];
            this.isLoading = false;
            this.skip += 10;
        });
    }

    scrollToTop(): void {
        window.scrollTo({top: 0, behavior: 'smooth'});
    }

    highlight(text: string, term: string): string {
        if (!term) return text;
        const regex = new RegExp(`(${term})`, 'gi');
        return text.replace(regex, `<span class="highlight">$1</span>`);
    }

}
