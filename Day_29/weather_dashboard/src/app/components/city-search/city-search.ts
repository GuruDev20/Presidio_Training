import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { WeatherService } from '../../services/weather';

@Component({
    selector: 'app-city-search',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './city-search.html',
    styleUrls: ['./city-search.css'],
})
export class CitySearchComponent {
    cityName = '';

    constructor(private weatherService: WeatherService) {}

    searchWeather() {
        if (this.cityName.trim()) {
            this.weatherService.fetchWeather(this.cityName);
        }
    }
}
