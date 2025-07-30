import { CommonModule } from "@angular/common";
import { Component, OnDestroy, OnInit } from "@angular/core";
import { CardComponent } from "../../../components/cards/card.component";
import { AdminService } from "../../../services/admin.service";
import { LucideIconsModule } from "../../../utils/lucide-icons.module";
import { Activity, ChevronDown, Ticket, UserRound, Users } from "lucide-angular";
import { FormsModule } from "@angular/forms";
import { ChartConfiguration, ChartData, ChartType } from "chart.js";
import { interval, Subject, Subscription } from "rxjs";
import { debounceTime } from "rxjs/operators";
import { NgChartsModule } from 'ng2-charts';
import { Router } from "@angular/router";

@Component({
    selector: 'app-admin-overview',
    templateUrl: './overview.html',
    standalone: true,
    imports: [CommonModule, CardComponent, LucideIconsModule, FormsModule, NgChartsModule],
})
export class AdminDashboard implements OnInit, OnDestroy {
    chevronDown = ChevronDown;

    cards = [
        { title: 'Total Users', count: 0, bgColor: '#5fa8d3', icon: Users },
        { title: 'Active Users', count: 0, bgColor: '#a1cca5', icon: UserRound },
        { title: 'Active Agents', count: 0, bgColor: '#ffa69e', icon: Activity },
        { title: 'Total Raised Tickets', count: 0, bgColor: '#f7d794', icon: Ticket }
    ];

    pieChartOptions: ChartConfiguration["options"] = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { position: 'right' } },
    };

    pieChartData: ChartData<'doughnut'> = {
        labels: ['Total Users', 'Active Users', 'Total Agents', 'Active Agents'],
        datasets: [{ data: [0, 0, 0, 0], backgroundColor: ["#5fa8d3", "#a1cca5", "#ffa69e", "#f7d794"] }]
    };
    pieChartType: ChartType = 'doughnut';
    pieFilter: string = 'all';

    lineChartOptions: ChartConfiguration["options"] = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { position: 'bottom' } },
    };
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
    totalPages: number = 1;
    agents: any[] = [];

    private subscriptions: Subscription[] = [];
    private fetchSubject = new Subject<void>();

    constructor(private adminService: AdminService,private router: Router) {}

    ngOnInit() {
        this.subscriptions.push(
            this.fetchSubject.pipe(debounceTime(200)).subscribe(() => {
                this.fetchAllData();
            })
        );
        this.fetchSubject.next();
        const updateSub = interval(15000).subscribe(() => {
            this.fetchSubject.next();
        });
        this.subscriptions.push(updateSub);
    }

    ngOnDestroy(): void {
        this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    fetchAllData() {
        this.fetchOverviewData();
        this.fetchDeactivationRequests();
        this.fetchAgents();
        this.fetchTickets();
        this.updateLineChartData(this.lineFilter);
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
                        { title: 'Total Raised Tickets', count: data.totalTickets, bgColor: '#f7d794', icon: Ticket }
                    ];
                    this.pieChartData = {
                        labels: ['Total Users', 'Active Users', 'Total Agents', 'Active Agents'],
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
                } else {
                    console.error('Overview fetch failed:', response.message);
                }
            },
            error: (error) => {
                console.error('Error fetching overview data:', error);
            }
        });
    }

    updatePieChartData(filter: string) {
        this.pieFilter = filter;
        this.fetchSubject.next();
    }

    updateLineChartData(filter: string) {
        this.lineFilter = filter;
        this.adminService.getTicketGrowth(filter).subscribe({
            next: (response) => {
                if (response.success) {
                    this.lineChartData = {
                        labels: response.data.labels.$values,
                        datasets: [{ data: response.data.values.$values, label: "Tickets Raised", borderColor: "#5fa8d3", fill: false }]
                    };
                } else {
                    console.error('Ticket growth fetch failed:', response.message);
                }
            },
            error: (error) => {
                console.error("Error fetching ticket growth data:", error);
            }
        });
    }

    fetchDeactivationRequests() {
        this.adminService.getDeactivationRequests().subscribe({
            next: (response) => {
                if (response.success) {
                    this.deactivationRequests = response.data.$values;
                } else {
                    console.error('Deactivation requests fetch failed:', response.message);
                }
            },
            error: (error) => {
                console.error('Error fetching deactivation requests:', error);
            }
        });
    }

    fetchAgents() {
        this.adminService.getAgentDetails().subscribe({
            next: (response) => {
                if (response.success) {
                    this.agents = response.data.$values || [];
                } else {
                    console.error('Agents fetch failed:', response.message);
                    this.agents = [];
                }
            },
            error: (error) => {
                console.error('Error fetching agents:', error.message || error);
                this.agents = [];
            }
        });
    }

    fetchTickets() {
        this.adminService.getTicketDetails(this.currentPage, this.pageSize).subscribe({
            next: (response) => {
                if (response.success) {
                    this.tickets = response.data.tickets.$values;
                    this.totalTickets = response.data.totalCount;
                    this.totalPages = Math.ceil(this.totalTickets / this.pageSize) || 1;
                } else {
                    console.error('Tickets fetch failed:', response.message);
                }
            },
            error: (error) => {
                console.error('Error fetching tickets:', error);
            }
        });
    }

    changePage(page: number) {
        if (page >= 1 && page <= this.totalPages) {
            this.currentPage = page;
            this.fetchSubject.next();
        }
    }

    manage(){
        this.router.navigate([`/admin/dashboard/workspace/manage-agents`]);
    }

    viewAllDeactivationRequests() {
        this.router.navigate(['/admin/dashboard/workspace/manage-users']);
    }
}