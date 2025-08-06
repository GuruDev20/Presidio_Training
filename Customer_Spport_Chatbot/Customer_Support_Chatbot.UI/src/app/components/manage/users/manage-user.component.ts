import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { AdminService } from "../../../services/admin.service";

@Component({
    selector: 'app-manage-user',
    templateUrl: './manage-user.component.html',
    standalone: true,
    imports: [CommonModule]
})
export class ManageUserComponent implements OnInit {
    deactivationRequests: any[] = [];

    constructor(private adminService: AdminService) {}

    ngOnInit(): void {
        this.fetchDeactivationRequests();
    }

    fetchDeactivationRequests() {
        this.adminService.getDeactivationRequests().subscribe({
            next: (response) => {
                console.log('Deactivation requests fetched:', response);
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

    deleteRequest(requestId: string, userId: string) {
        console.log('Deleting deactivation request with ID:', requestId);
        console.log('User ID for deletion:', userId);
        this.adminService.updateDeactivationRequestStatus(userId, 'Deleted').subscribe({
            next: (response) => {
                if (response.success) {
                    this.deactivationRequests = this.deactivationRequests.map(request => 
                        request.id === requestId ? { ...request, status: 'Deleted' } : request
                    );
                    console.log('Deactivation request marked as deleted:', response);
                } else {
                    console.error('Failed to mark deactivation request as deleted:', response.message);
                }
            },
            error: (error) => {
                console.error('Error marking deactivation request as deleted:', error);
            }
        });
    }
}