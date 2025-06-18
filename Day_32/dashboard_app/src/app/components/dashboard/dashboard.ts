import { CommonModule, NgFor, NgIf } from '@angular/common';
import { AfterViewInit, Component, inject, OnDestroy, OnInit } from '@angular/core';
import { UserService } from '../../services/user';
import { User } from '../../models/user.model';
import { FormsModule } from '@angular/forms';
import { ChartData } from 'chart.js';
import { NgChartsModule } from 'ng2-charts';
import * as am4core from "@amcharts/amcharts4/core";
import * as am4maps from "@amcharts/amcharts4/maps";
import am4geodata_usaLow from "@amcharts/amcharts4-geodata/usaLow";

@Component({
    selector: 'app-dashboard',
    imports: [CommonModule,FormsModule,NgChartsModule],
    templateUrl: './dashboard.html',
    styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit{
    
    private userService=inject(UserService);

    users:User[] = [];
    filteredUsers: User[] = [];
    paginatedUsers: User[] = [];

    filters={gender:'', role: '', country: ''};
    currentPage = 1;
    pageSize = 10;
    totalPages=1;
    totalActiveUsers = 0;
    totalUsers = 0;
    totalAdmins = 0;

    pieChartLables:string[]=['Male','Female'];
    pieChartData:ChartData<'pie',number[]>={
        labels:this.pieChartLables,
        datasets:[{data:[0,0]}]
    }

    barChartLabels:string[]=['Admin', 'User', 'Moderator'];
    barChartData:ChartData<'bar', number[]> = {
        labels: this.barChartLabels,
        datasets: [{ data: [0, 0, 0], label: 'Users by Role' }]
    }

    mapChart!: am4maps.MapChart;
    polygonSeries!: am4maps.MapPolygonSeries;
    ngOnInit(): void {
        this.createMap();
        this.userService.getUsers().subscribe();
        this.userService.users.subscribe(users => {
            this.users = users;
            this.applyFilters();
        });
    }

    applyFilters(){
        const {gender, role, country} = this.filters;
        this.userService.applyFilters(this.filters);
        this.filteredUsers = this.users.filter(user=>{
            const userGender = user.gender?.toLowerCase() || '';
            const userRole = user.role?.toLowerCase() || '';
            const userState = user.address?.state?.toLowerCase() || '';

            return (!gender || userGender === gender.toLowerCase()) &&
                (!role || userRole.includes(role.toLowerCase())) &&
                (!country || userState.includes(country.toLowerCase()));
        });

        this.totalActiveUsers = this.filteredUsers.length;
        this.totalAdmins = this.filteredUsers.filter(user => user.role.toLowerCase() === 'admin').length;
        this.totalUsers=this.filteredUsers.filter(user => user.role.toLowerCase() === 'user').length;

        this.updatePieChart();
        this.updateBarChart();
        this.paginate();
        this.updateMapStates(this.filteredUsers);
    }

    updatePieChart(){
        const male= this.filteredUsers.filter(user => user.gender === 'male').length;
        const female= this.filteredUsers.filter(user => user.gender === 'female').length;
        this.pieChartData={
            labels: this.pieChartLables,
            datasets: [{ data: [male, female]}]
        };
    }

    updateBarChart(){
        const roles=['admin', 'user', 'moderator'];
        const data=roles.map(role=>
            this.filteredUsers.filter(user => user.role.toLowerCase() === role).length
        );
        this.barChartData={
            labels: this.barChartLabels,
            datasets: [{ data, label: 'Users by Role' }]
        }
    }

    paginate(){
        this.totalPages=Math.ceil(this.filteredUsers.length/this.pageSize);
        const start= (this.currentPage - 1) * this.pageSize;
        const end=start + this.pageSize;
        this.paginatedUsers = this.filteredUsers.slice(start, end);
    }

    nextPage(){
        if (this.currentPage < this.totalPages) {
            this.currentPage++;
            this.paginate();
        }
    }

    prevPage(){
        if (this.currentPage > 1) {
            this.currentPage--;
            this.paginate();
        }
    }

    createMap() {
        const chart = am4core.create("usMap", am4maps.MapChart);
        chart.geodata = am4geodata_usaLow;
        chart.projection = new am4maps.projections.AlbersUsa();

        const polygonSeries = chart.series.push(new am4maps.MapPolygonSeries());
        polygonSeries.useGeodata = true;
        polygonSeries.mapPolygons.template.tooltipText = "{name}: {value}";

        polygonSeries.heatRules.push({
            property: "fill",
            target: polygonSeries.mapPolygons.template,
            min: am4core.color("#E5ECF6"),
            max: am4core.color("#367BCE")
        });

        this.mapChart = chart;
        this.polygonSeries = polygonSeries;
    }

    updateMapStates(users: User[]) {
        const stateCounts = users.reduce((acc, user) => {
            const state = user.address?.state;
            if (state) acc[state] = (acc[state] || 0) + 1;
            return acc;
        }, {} as Record<string, number>);

        this.polygonSeries.data = Object.entries(stateCounts).map(([state, count]) => ({
            id: "US-" + this.getStateCode(state),
            value: count
        }));
    }

    getStateCode(stateName: string): string {
        const stateMap: Record<string, string> = {
            'Alabama': 'AL', 'Alaska': 'AK', 'Arizona': 'AZ', 'Arkansas': 'AR', 'California': 'CA',
            'Colorado': 'CO', 'Connecticut': 'CT', 'Delaware': 'DE', 'Florida': 'FL', 'Georgia': 'GA',
            'Hawaii': 'HI', 'Idaho': 'ID', 'Illinois': 'IL', 'Indiana': 'IN', 'Iowa': 'IA',
            'Kansas': 'KS', 'Kentucky': 'KY', 'Louisiana': 'LA', 'Maine': 'ME', 'Maryland': 'MD',
            'Massachusetts': 'MA', 'Michigan': 'MI', 'Minnesota': 'MN', 'Mississippi': 'MS', 'Missouri': 'MO',
            'Montana': 'MT', 'Nebraska': 'NE', 'Nevada': 'NV', 'New Hampshire': 'NH', 'New Jersey': 'NJ',
            'New Mexico': 'NM', 'New York': 'NY', 'North Carolina': 'NC', 'North Dakota': 'ND', 'Ohio': 'OH',
            'Oklahoma': 'OK', 'Oregon': 'OR', 'Pennsylvania': 'PA', 'Rhode Island': 'RI', 'South Carolina': 'SC',
            'South Dakota': 'SD', 'Tennessee': 'TN', 'Texas': 'TX', 'Utah': 'UT', 'Vermont': 'VT',
            'Virginia': 'VA', 'Washington': 'WA', 'West Virginia': 'WV', 'Wisconsin': 'WI', 'Wyoming': 'WY'
        };
        return stateMap[stateName] || '';
    }
}
