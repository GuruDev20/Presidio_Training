import { CommonModule } from "@angular/common";
import { Component, OnDestroy, OnInit } from "@angular/core";
import { Subscription } from "rxjs";
import { SignalRService } from "../../services/signalr.service";
import { Router } from "@angular/router";

@Component({
  selector: 'app-active-workspace',
  templateUrl: './active.component.html',
  standalone: true,
  imports: [CommonModule]
})
export class ActiveWorkspaceComponent implements OnInit,OnDestroy {

    tickets:any[]=[];
    private signalrSub!:Subscription;

    constructor(private signalRService:SignalRService,private router:Router){}

    ngOnInit(): void {
        this.signalRService.startConnection();

        this.signalrSub=this.signalRService.ticketNotification$.subscribe(data=>{
            if(data){
                this.tickets.push(data);
            }
        })
    }

    ngOnDestroy():void{
        if(this.signalrSub){
            this.signalrSub.unsubscribe();
        }
        this.signalRService.stopConnection();
    }

    joinChat(ticketId: string) {
        this.router.navigate(['/chat'], {
            queryParams: {
                ticketId,
                agent: true
            }
        });
    }  

}
