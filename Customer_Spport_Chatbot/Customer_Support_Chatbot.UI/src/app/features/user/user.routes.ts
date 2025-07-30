import { Routes } from "@angular/router";
import { Overview } from "../shared/overview/overview";
import { UserLayoutComponent } from "../../layout/user-layout/user-layout.component";
import { User_Sidebar_Constants } from "../../constants/user-sidebar.constant";
import { UserHistoryComponent } from "./history/history.component";
import { Profile } from "../../components/profile/profile.component";
import { SettingsComponent } from "../settings/settings.component";
import { UserSettingsComponent } from "../../components/settings/user-settings/user-settings.component";

export const User_Routes:Routes=[
    {
        path:'dashboard',
        component:UserLayoutComponent,
        data:{items:User_Sidebar_Constants},
        children:[
            {path:'', redirectTo:'overview', pathMatch:'full'},
            {path:'overview',component:Overview},
            {
                path:'tickets',
                loadComponent:()=>import('./tickets/tickets.component').then(m=>m.UserTicketsComponent),
                children:[
                    {path:'new',loadComponent:()=>import('../../components/ticket/new-ticket.component').then(m=>m.NewTicketComponent)},
                    {path:'active',loadComponent:()=>import('../../components/ticket/active-ticket.component').then(m=>m.ActiveTicketComponent)},
                    {path:'chat',loadComponent:()=>import('../../components/chat/chat.component').then(m=>m.ChatComponent)},
                ]
            },
            {path:'history',component:UserHistoryComponent},
            {path:'pricing', loadComponent: () => import('../pricing/pricing').then(m => m.Pricing)},
            {path:'profile',component:Profile},
            {path:'settings',component:UserSettingsComponent}
        ]
    }
]