import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Sidebar } from "./components/sidebar/sidebar";
import { NgChartsModule } from 'ng2-charts';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet, Sidebar,NgChartsModule],
    templateUrl: './app.html',
    styleUrl: './app.css',
    standalone: true
})
export class App {
    protected title = 'dashboard_app';
}
