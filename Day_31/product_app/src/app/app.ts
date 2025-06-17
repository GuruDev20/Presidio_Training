import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet,CommonModule],
    templateUrl: './app.html',
    styleUrl: './app.css',
    standalone: true
})
export class App {
    protected title = 'product_app';
}
