import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { CardComponent } from "../../../components/cards/card.component";

@Component({
    selector: 'app-overview',
    templateUrl: './overview.component.html',
    standalone: true,
    imports: [CommonModule,CardComponent],
})

export class OverviewComponent{

    openMenuIndex:number|null=null;

    cards=[
        {title:'Total Tickets',count:120,bgColor:'#5fa8d3'},
        {title:'Active Tickets',count:45,bgColor:'#a1cca5'},
        {title:'Closed Tickets',count:75,bgColor:'#ffa69e'},
    ];

    history=[
        {title:'Issue with login',status:'Active'},
        {title:'Payment not received',status:'Closed'},
        {title:'Feature request',status:'Active'},
        {title:'Bug in application',status:'Pending'},
        {title:'Account verification',status:'Closed'},
        {title:'Password reset',status:'Pending'},
        {title:'Subscription issue',status:'Active'},
        {title:'Feedback on new feature',status:'Closed'},
        {title:'Report a bug',status:'Active'},
        {title:'Request for support',status:'Closed'},
        {title:'Inquiry about service',status:'Pending'},
    ];

    currentPage=1;
    itemsPerPage=8;

    constructor(private router:Router){}

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