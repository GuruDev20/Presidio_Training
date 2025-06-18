import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faUserPlus } from '@fortawesome/free-solid-svg-icons/faUserPlus';
import { faChartPie } from '@fortawesome/free-solid-svg-icons/faChartPie';

@Component({
    selector: 'app-sidebar',
    imports: [RouterModule,CommonModule,FontAwesomeModule],
    templateUrl: './sidebar.html',
    styleUrl: './sidebar.css',
    standalone: true
})
export class Sidebar {
    faUserPlus=faUserPlus;
    faChartPie=faChartPie;
}
