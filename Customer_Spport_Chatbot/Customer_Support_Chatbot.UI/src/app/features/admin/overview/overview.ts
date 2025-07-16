import { CommonModule } from "@angular/common";
import { Component, OnDestroy, OnInit } from "@angular/core";
import { CardComponent } from "../../../components/cards/card.component";
import { AdminService } from "../../../services/admin.service";
import { LucideIconsModule } from "../../../utils/lucide-icons.module";
import { Activity, ChevronDown, Ticket, UserRound, Users } from "lucide-angular";
import { FormsModule } from "@angular/forms";
import { ChartConfiguration, ChartData, ChartType } from "chart.js";
import { interval, Subscription } from "rxjs";
import { NgChartsModule } from 'ng2-charts';

@Component({
    selector: 'app-admin-overview',
    templateUrl: './overview.html',
    standalone: true,
    imports: [CommonModule, CardComponent,LucideIconsModule,FormsModule, NgChartsModule],
})

export class AdminDashboard implements OnInit,OnDestroy {

    chevronDown = ChevronDown;

    cards = [
        { title: 'Total Users', count: 0, bgColor: '#5fa8d3',icon:Users },
        { title: 'Active Users', count: 0, bgColor: '#a1cca5',icon:UserRound },
        { title: 'Active Agents', count: 0, bgColor: '#ffa69e',icon:Activity },
        { title: 'Total Raised Tickets', count: 0, bgColor: '#f7d794',icon:Ticket }
    ];

    pieChartOptions:ChartConfiguration["options"]={
        responsive:true,
        maintainAspectRatio:false,
        plugins: {legend: { position: 'right' }},
    }

    pieChartData: ChartData<'doughnut'> = {
        labels: ['Total Users', 'Active Users','Total Agents, Active Agents'],
        datasets: [{ data: [0, 0, 0, 0], backgroundColor: ["#5fa8d3", "#a1cca5", "#ffa69e", "#f7d794"] }]
    };
    pieChartType: ChartType = 'doughnut';
    pieFilter: string = 'all';

    lineChartOptions: ChartConfiguration["options"] = {
        responsive: true,
        maintainAspectRatio: false,
        plugins:{legend: { position: 'bottom' }},
    }
    lineChartData: ChartData<"line"> = {
        labels: [],
        datasets: [{ data: [], label: "Tickets Raised", borderColor: "#5fa8d3", fill: false }]
    };
    lineChartType: ChartType = "line";
    lineFilter: string = "last24hours";

    deactivationRequests: any[] = [];
    agentFilter: string = "online";
    tickets: any[] = [];
    currentPage: number = 1;
    pageSize: number = 5;
    totalTickets: number = 0;

    private subscriptions: Subscription[] = [];

    constructor(private adminService: AdminService) {}

    ngOnInit() {
        this.fetchOverviewData();
        this.fetchDeactivationRequests();
        this.fetchTickets();
        this.updateLineChartData(this.lineFilter); 
        const updateSub=interval(30000).subscribe(()=>{
            this.fetchOverviewData();
            this.fetchDeactivationRequests();
            this.fetchTickets();
            this.updateLineChartData(this.lineFilter); 
        })
        this.subscriptions.push(updateSub);
    }

    ngOnDestroy(): void {
        this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    fetchOverviewData() {
        this.adminService.getOverview().subscribe({
            next: (response) => {
                if (response.success) {
                    const data = response.data;
                    this.cards = [
                        { title: 'Total Users', count: data.totalUsers, bgColor: '#5fa8d3', icon: Users },
                        { title: 'Active Users', count: data.activeUsers, bgColor: '#a1cca5', icon: UserRound },
                        { title: 'Active Agents', count: data.activeAgents, bgColor: '#ffa69e', icon: Activity },
                        { title: 'Total Raised Tickets', count: data.totalTickets, bgColor: '#f7d794',icon:Ticket }
                    ];
                    this.pieChartData = {
                        labels: ['Total Users', 'Active Users','Total Agents', 'Active Agents'],
                            datasets: [{
                                data: [
                                    data.totalUsers || 0,
                                    data.activeUsers || 0,
                                    data.totalAgents || 0,
                                    data.activeAgents || 0
                                ],
                                backgroundColor: ['#5fa8d3', '#a1cca5', '#ffa69e', '#f7d794']
                            }]
                    };
                }
            },
            error: (error) => {
                console.error('Error fetching overview data:', error);
            }
        });
    }

    updatePieChartData(filter: string) {
        this.pieFilter = filter;

        this.adminService.getOverview().subscribe({
            next: (response) => {
                if (response.success) {
                    const data = response.data;
                    let labels: string[] = [];
                    let values: number[] = [];
                    let bgColors: string[] = [];

                    if (filter === 'all') {
                        labels = ['Total Users', 'Active Users', 'Total Agents', 'Active Agents'];
                        values = [data.totalUsers, data.activeUsers,data.totalAgents, data.activeAgents];
                        bgColors = ["#5fa8d3", "#a1cca5", "#ffa69e", "#f7d794"];
                    } 
                    else if (filter === 'users') {
                        labels = ['Total Users', 'Active Users'];
                        values = [data.totalUsers, data.activeUsers];
                        bgColors = ['#5fa8d3', '#a1cca5'];
                    } 
                    else if (filter === 'agents') {
                        labels = ['Total Agents', 'Active Agents'];
                        values = [data.totalAgents, data.activeAgents];
                        bgColors = ['#ffa69e', '#f7d794'];
                    }
                    this.pieChartData = {
                        labels: labels,
                        datasets: [{
                            data: values,
                            backgroundColor: bgColors
                        }]
                    };
                }
            },
            error: (error) => {
                console.error('Error updating pie chart data:', error);
            }
        });
    }

    updateLineChartData(filter: string){
        this.lineFilter = filter;
        this.adminService.getTicketGrowth(filter).subscribe({
            next: (response) => {
                if (response.success) {
                    this.lineChartData = {
                        labels: response.data.labels.$values,
                        datasets: [{ data: response.data.values.$values, label: "Tickets Raised", borderColor: "#5fa8d3", fill: false }]
                    };
                }
            },
            error: (error) => {
                console.error("Error fetching ticket growth data:", error);
            }
        });
    }

    fetchDeactivationRequests(){}

    fetchTickets(){}

}