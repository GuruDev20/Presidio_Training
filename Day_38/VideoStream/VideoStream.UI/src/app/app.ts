import { Component } from '@angular/core';
import { Upload } from "./components/upload/upload";
import { List } from "./components/list/list";
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-root',
    templateUrl: './app.html',
    styleUrl: './app.css',
    imports: [Upload, List,CommonModule]
})
export class App {
    mode:'upload' | 'view' = 'upload';
}
