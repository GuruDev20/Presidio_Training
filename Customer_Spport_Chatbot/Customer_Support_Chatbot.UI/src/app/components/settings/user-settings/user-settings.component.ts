import { Component } from "@angular/core";
import { LucideIconsModule } from "../../../utils/lucide-icons.module";
import { CommonModule } from "@angular/common";

@Component({
    selector: 'app-user-settings',
    templateUrl: './user-settings.component.html',
    standalone: true,
    imports:[LucideIconsModule,CommonModule]
})

export class UserSettingsComponent{}