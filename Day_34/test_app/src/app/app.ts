import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { RecipeParent } from "./components/recipe-parent/recipe-parent";

@Component({
  selector: 'app-root',
  imports: [RecipeParent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'test_app';
}
