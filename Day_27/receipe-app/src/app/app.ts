import { Component } from '@angular/core';
import { RecipeListComponent } from "./components/recipe-list/recipe-list";

@Component({
    selector: 'app-root',
    imports: [RecipeListComponent],
    templateUrl: './app.html',
    styleUrl: './app.css'
})
export class App {
    protected title = 'receipe-app';
}
