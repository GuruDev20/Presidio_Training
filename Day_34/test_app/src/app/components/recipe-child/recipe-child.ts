import { Component, Input } from '@angular/core';
import { Recipe } from '../../models/recipe';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-recipe-child',
    imports: [CommonModule],
    templateUrl: './recipe-child.html',
    styleUrl: './recipe-child.css',
    standalone: true
})
export class RecipeChild {
    @Input() recipe!:Recipe;
}
