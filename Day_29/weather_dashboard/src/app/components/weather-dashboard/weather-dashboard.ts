import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CitySearchComponent } from '../city-search/city-search';
import { WeatherCardComponent } from '../weather-card/weather-card';
import { AsyncPipe } from '@angular/common';
import { WeatherService } from '../../services/weather';
import { Observable } from 'rxjs';

@Component({
    selector: 'app-weather-dashboard',
    standalone: true,
    imports: [CommonModule, CitySearchComponent, WeatherCardComponent, AsyncPipe],
    templateUrl: './weather-dashboard.html',
})
export class WeatherDashboardComponent {
    weather$: Observable<any>;
    error$: Observable<string | null>;

    constructor(private weatherService: WeatherService) {
        this.weather$ = this.weatherService.weather$;
        this.error$ = this.weatherService.error$;
    }
}
