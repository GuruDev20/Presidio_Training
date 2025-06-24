import { Routes } from '@angular/router';
import { AuthGurad } from './guards/auth.guard';
import { AdminDashboard } from './features/admin/dashboard/dashboard';
import { AgentDashboard } from './features/agent/dashboard/dashboard';
import { Auth_Routes } from './features/auth/auth.route';
import { UserLayout } from './layout/user-layout/user-layout.component';

export const routes: Routes = [

    // Auth routes
    {path:'auth',children:Auth_Routes},

    //Role based Dashboard routes
    {
        path:'user/dashboard',
        component:UserLayout,
        canActivate:[AuthGurad],
        children:[
            {path:'',redirectTo:'overview',pathMatch:'full'},
            {path:'overview',loadComponent: () => import('./features/user/overview/overview.component').then(m => m.OverviewComponent) },
            {path:'tickets',loadComponent: () => import('./features/user/tickets/tickets.component').then(m => m.TicketsComponent) },
            {path:'history',loadComponent: () => import('./features/user/history/history.component').then(m => m.HistoryComponent) }
        ]
    },
    {path:'admin/dashboard',component:AdminDashboard,canActivate:[AuthGurad]},
    {path:'agent/dashboard',component:AgentDashboard,canActivate:[AuthGurad]},

    // Fallback route
    {path:'',redirectTo:'auth/login',pathMatch:'full'}
];
