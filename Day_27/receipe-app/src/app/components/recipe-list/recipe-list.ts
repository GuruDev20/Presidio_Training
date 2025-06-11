import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RecipeService } from '../../service/receipe.service';
import { RecipeModel } from '../../models/receipe';

@Component({
    selector: 'app-recipe-list',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './recipe-list.html',
    styleUrls: ['./recipe-list.css']
})
export class RecipeListComponent implements OnInit {
    recipes = signal<RecipeModel[]>([]);

    constructor(private recipeService: RecipeService) {}

    ngOnInit(): void {
        // this.recipeService.getAllRecipes();
        this.recipes = this.recipeService.recipes;
    }
}
