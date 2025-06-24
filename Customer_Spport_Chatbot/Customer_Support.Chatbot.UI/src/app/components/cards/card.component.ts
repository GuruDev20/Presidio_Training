import { CommonModule } from "@angular/common";
import { Component, Input, OnChanges, SimpleChanges } from "@angular/core";

@Component({
    selector: 'app-card',
    templateUrl: './card.component.html',
    standalone: true,
    imports: [CommonModule],
})
export class CardComponent implements OnChanges{
    @Input() title:string = '';
    @Input() count:number = 0;
    @Input() bgColor:string = '#ffffff';

    animatedCount:number = 0;

    ngOnChanges(changes: SimpleChanges): void {
        if(changes['count']){
            this.animateCount(0,this.count,800);
        }
    }

    animateCount(start:number,end:number,duration:number){
        const startTime=performance.now();
        const animate=(currentTime:number)=>{
            const elpsed=currentTime-startTime;
            const progress=Math.min(elpsed/duration,1);
            this.animatedCount=Math.floor(start + (end - start) * progress);
            if(progress<1){
                requestAnimationFrame(animate);
            }
            else{
                this.animatedCount=end;
            }
        };
        requestAnimationFrame(animate);
    }
}