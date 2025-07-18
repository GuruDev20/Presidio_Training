import { Component, OnInit } from "@angular/core";
import { AuthService } from "../../services/auth.service";
import { UserSettingsComponent } from "../../components/settings/user-settings/user-settings.component";
import { AgentSettingsComponent } from "../../components/settings/agent-settings/agent-settings.component";
import { AdminSettingsComponent } from "../../components/settings/admin-settings/admin-settings.component";
import { CommonModule } from "@angular/common";

@Component({
    selector:'app-settings',
    templateUrl:'./settings.component.html',
    standalone:true,
    imports:[UserSettingsComponent,AgentSettingsComponent,AdminSettingsComponent,CommonModule]
})

export class SettingsComponent implements OnInit{

    userRole:string|null=null;

    constructor(private authService:AuthService){}

    ngOnInit(): void {
        this.userRole = this.authService.getRole();
    }
}