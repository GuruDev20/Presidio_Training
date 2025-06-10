import { Component } from '@angular/core';

@Component({
    selector: 'app-customer-details',
    templateUrl: './customer-details.html',
    styleUrl: './customer-details.css',
})
export class CustomerDetailsComponent {
     customerName = 'Dev';
     customerEmail = 'dev@gmail.com';
     likeCount = 0;
     dislikeCount = 0;

     like() {
          this.likeCount++;
     }

     dislike() {
          this.dislikeCount++;
     }
}