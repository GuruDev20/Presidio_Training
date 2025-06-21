import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-root',
    templateUrl: './app.html',
    styleUrl: './app.css',
    imports: [RouterOutlet],
    standalone: true
})
export class App {
    protected title = 'Customer_Support.Chatbot.UI';
}
