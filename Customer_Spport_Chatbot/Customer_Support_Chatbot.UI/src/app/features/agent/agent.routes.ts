import { Routes } from "@angular/router";
import { AgentLayoutComponent } from "../../layout/agent-layout/agent-layout.component";
import { Agent_Sidebar_Constants } from "../../constants/agent-sidebar.constant";
import { AgentWorkspaceComponent } from "./workspace/workspace.component";
import { AgentHistoryComponent } from "./history/history.component";
import { Overview } from "../shared/overview/overview";

export const Agent_Routes:Routes=[
    {
        path:'dashboard',
        component:AgentLayoutComponent,
        data:{items:Agent_Sidebar_Constants},
        children:[
            {path:'', redirectTo:'overview', pathMatch:'full'},
            {path:'overview',component:Overview},
            {
                path:'workspace',
                component:AgentWorkspaceComponent,
                children:[
                    {path:'',redirectTo:'active',pathMatch:'full'},
                    {path:'active',loadComponent:()=>import('../../components/workspace/active.component').then(m=>m.ActiveWorkspaceComponent)},
                    {path:'pending',loadComponent:()=>import('../../components/workspace/pending.component').then(m=>m.PendingWorkspaceComponent)},
                    {path:'chat',loadComponent:()=>import('../../components/chat/chat.component').then(m=>m.ChatComponent)},
                ]
            },
            {path:'history',component:AgentHistoryComponent},
        ]
    }
]