import { CommonModule } from "@angular/common";
import { Component, Input } from "@angular/core";

@Component({
    selector: 'app-card',
    templateUrl: './card.component.html',
    standalone: true,
    imports: [CommonModule],
})
export class CardComponent{
    @Input() title:string = '';
    @Input() count:number = 0;
    @Input() bgColor:string = '#ffffff';
}