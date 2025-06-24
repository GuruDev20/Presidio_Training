import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastComponent } from "./components/toast/toast";
import { ToastService } from './services/toast.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.html',
    styleUrl: './app.css',
    imports: [RouterOutlet, ToastComponent],
    standalone: true
})
export class App implements AfterViewInit{
    @ViewChild('toast') toastComponent!: ToastComponent;

    constructor(private toastService: ToastService) {}

    ngAfterViewInit(): void {
        this.toastService.register(this.toastComponent);
    }
}
