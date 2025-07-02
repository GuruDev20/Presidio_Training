import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { CardComponent } from "../../../components/cards/card.component";
import { AdminService } from "../../../services/admin.service";

@Component({
    selector: 'app-admin-overview',
    templateUrl: './overview.html',
    standalone: true,
    imports: [CommonModule, CardComponent]
})
export class AdminDashboard implements OnInit {
    cards = [
        { title: 'Total Users', count: 0, bgColor: '#5fa8d3' },
        { title: 'Active Users', count: 0, bgColor: '#a1cca5' },
        { title: 'Active Agents', count: 0, bgColor: '#ffa69e' },
        { title: 'Total Raised Tickets', count: 0, bgColor: '#f7d794' }
    ];

    constructor(private adminService: AdminService) {}

    ngOnInit() {
        this.fetchOverviewData();
    }

    fetchOverviewData() {
        this.adminService.getOverview().subscribe({
            next: (response) => {
                if (response.success) {
                    const data = response.data;
                    this.cards = [
                        { title: 'Total Users', count: data.totalUsers, bgColor: '#5fa8d3' },
                        { title: 'Active Users', count: data.activeUsers, bgColor: '#a1cca5' },
                        { title: 'Active Agents', count: data.activeAgents, bgColor: '#ffa69e' },
                        { title: 'Total Raised Tickets', count: data.totalTickets, bgColor: '#f7d794' }
                    ];
                }
            },
            error: (error) => {
                console.error('Error fetching overview data:', error);
            }
        });
    }
}