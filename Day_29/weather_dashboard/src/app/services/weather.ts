import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError, interval, switchMap, catchError } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class WeatherService {
    private apiKey = 'd2c8fe4b2eea480696d1c9a827184169'; 
    private weatherSubject = new BehaviorSubject<any>(null);
    weather$ = this.weatherSubject.asObservable();

    private errorSubject = new BehaviorSubject<string | null>(null);
    error$ = this.errorSubject.asObservable();

    constructor(private http: HttpClient) {}

    fetchWeather(city: string): void {
        const url = `https://api.openweathermap.org/data/2.5/weather?q=${city}&appid=${this.apiKey}&units=metric`;

        this.http.get(url).pipe(
        catchError(err => {
            const errorMsg = err.status === 404 ? 'City not found' : 'API error occurred';
            this.errorSubject.next(errorMsg);
            return throwError(() => new Error(errorMsg));
        })
        ).subscribe(data => {
            this.errorSubject.next(null);
            this.weatherSubject.next(data);
        });
  }
    startAutoRefresh(city: string) {
        interval(300000).pipe(
            switchMap(() => this.http.get(`https://api.openweathermap.org/data/2.5/weather?q=${city}&appid=${this.apiKey}&units=metric`))
            ).subscribe(data => {
                this.weatherSubject.next(data);
        });
    }
}
