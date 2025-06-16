import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({providedIn: 'root'})
export class ProductService{

    constructor(private http: HttpClient) {}

    searchProducts(query:string,skip:number=0):Observable<any>{
        const url = `https://dummyjson.com/products/search?q=${query}&limit=10&skip=${skip}`;
        return this.http.get(url);
    }
}