import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Menu } from './components/menu/menu';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet, Menu],
    templateUrl: './app.html',
    styleUrl: './app.css',
    standalone: true
})
export class App {
    protected title = 'product_list';
}
