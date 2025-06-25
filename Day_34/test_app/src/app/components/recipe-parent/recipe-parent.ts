import { Component, OnInit } from '@angular/core';
import {RecipeService } from '../../services/recipe';
import { Recipe } from '../../models/recipe';
import { RecipeChild } from "../recipe-child/recipe-child";
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-recipe-parent',
    imports: [RecipeChild,CommonModule],
    templateUrl: './recipe-parent.html',
    styleUrl: './recipe-parent.css',
    standalone: true
})
export class RecipeParent implements OnInit{
    recipes:Recipe[]=[];
    constructor(private recipeService:RecipeService){}

    ngOnInit(): void {
        this.recipeService.getRecipes().subscribe(res=>{
            this.recipes = res.recipes;
        });
    }
}
