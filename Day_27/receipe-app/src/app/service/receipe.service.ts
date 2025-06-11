import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { RecipeModel } from '../models/receipe';

@Injectable()
export class RecipeService {
    private http = inject(HttpClient);
    recipes = signal<RecipeModel[]>([]);

    getAllRecipes(): void {
        this.http.get<{ recipes: RecipeModel[] }>('https://dummyjson.com/recipes')
        .pipe(
            catchError(error => {
                console.error('Error fetching recipes', error);
                return throwError(() => error);
            })
        )
        .subscribe(response => {
            this.recipes.set(response.recipes);
        });
    }
}
