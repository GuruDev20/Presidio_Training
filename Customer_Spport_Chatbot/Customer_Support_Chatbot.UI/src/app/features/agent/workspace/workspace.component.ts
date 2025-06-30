import { Component } from "@angular/core";
import { ActivatedRoute, Router, RouterModule } from "@angular/router";
import { LucideIconsModule } from "../../../utils/lucide-icons.module";
import { CommonModule } from "@angular/common";

@Component({
    selector:'app-agent-workspace',
    templateUrl:'./workspace.component.html',
    standalone:true,
    imports:[LucideIconsModule,CommonModule,RouterModule]
})

export class AgentWorkspaceComponent{}