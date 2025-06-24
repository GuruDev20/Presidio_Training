import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { CardComponent } from "../../../components/cards/card.component";
import { AuthService } from "../../../services/auth.service";
import { UserService } from "../../../services/user.service";

@Component({
    selector: 'app-overview',
    templateUrl: './overview.component.html',
    standalone: true,
    imports: [CommonModule,CardComponent],
})

export class OverviewComponent implements OnInit{

    openMenuIndex:number|null=null;

    history:any[]=[]

    currentPage=1;
    itemsPerPage=8;

    constructor(private router:Router,private authService:AuthService,private userService:UserService){}

    ngOnInit(): void {
        const userId= this.authService.getUserId();
        const role= this.authService.getRole();
        this.userService.getTicketsByUser({userOrAgentId:userId,role})
            .subscribe({
                next:(res)=>{
                    this.history = Array.isArray((res.data as any)?.$values)
                        ? (res.data as any).$values
                        : [];
                    this.updateCardStats();
                },
                error:(err)=>{
                    console.error('Error fetching ticket history:', err);
                }
            })
    }

    cards=[
        { title: 'Total Tickets', count: 0, bgColor: '#5fa8d3' },
        { title: 'Active Tickets', count: 0, bgColor: '#a1cca5' },
        { title: 'Closed Tickets', count: 0, bgColor: '#ffa69e' },
    ];

    updateCardStats() {
        const total = this.history.length;
        const active = this.history.filter(h => h.status === 'Active').length;
        const closed = this.history.filter(h => h.status === 'Closed').length;

        this.cards[0].count = total;
        this.cards[1].count = active;
        this.cards[2].count = closed;
    }

    get totalPages():number{
        return Math.ceil(this.history.length / this.itemsPerPage);
    }

    get paginatedHistory(){
        const start=(this.currentPage - 1) * this.itemsPerPage;
        return this.history.slice(start, start + this.itemsPerPage);
    }
    nextPage(){
        if(this.currentPage < this.totalPages){
            this.currentPage++;
        }
    }

    prevPage(){
        if(this.currentPage > 1){
            this.currentPage--;
        }
    }

    viewMore(){
        this.router.navigate(['/user/dashboard/history']);
    }

    deleteHistory(index:number){
        const globalIndex=(this.currentPage-1)* this.itemsPerPage + index;
        this.history.splice(globalIndex, 1);
        this.openMenuIndex=null;
        if(this.currentPage>this.totalPages){
            this.currentPage = this.totalPages;
        }
    }

    toggleMenu(index:number){
        this.openMenuIndex= this.openMenuIndex === index ? null : index;
    }

    closeAllMenus(){
        this.openMenuIndex = null;
    }
}