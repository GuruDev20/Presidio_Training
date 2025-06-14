import { Component } from '@angular/core';
import { WeatherDashboardComponent } from './components/weather-dashboard/weather-dashboard';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [WeatherDashboardComponent],
    templateUrl: './app.html',
})
export class App {}
