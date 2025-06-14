import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-weather-card',
    templateUrl: './weather-card.html',
    styleUrls: ['./weather-card.css'],
})
export class WeatherCardComponent {
    @Input() data: any;
}
