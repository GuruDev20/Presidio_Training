import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { ProductModel, ProductResponse } from "../models/product.model";
import { Injectable } from "@angular/core";

@Injectable({providedIn: 'root'})
export class ProductService{
    private readonly baseUrl = 'https://dummyjson.com/products';

    constructor(private http: HttpClient) {}

    getProducts(query:string,skip:number,limit:number):Observable<ProductResponse>{
        const url = `${this.baseUrl}/search?q=${query}&limit=${limit}&skip=${skip}`;
        return this.http.get<ProductResponse>(url);
    }

    getproductsById(id:number):Observable<ProductModel>{
        const url = `${this.baseUrl}/${id}`;
        return this.http.get<ProductModel>(url);
    }
}