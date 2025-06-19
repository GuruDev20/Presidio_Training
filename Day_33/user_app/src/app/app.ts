import { Component } from '@angular/core';
import { UserDashboard } from "./components/user-dashboard/user-dashboard";

@Component({
    selector: 'app-root',
    imports: [UserDashboard],
    templateUrl: './app.html',
    styleUrl: './app.css'
})
export class App {
    protected title = 'user_app';
}
