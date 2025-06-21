import { Routes } from '@angular/router';
import { AuthGurad } from './core/guards/auth.guard';
import { UserDashboard } from './features/user/dashboard/dashboard';
import { AdminDashboard } from './features/admin/dashboard/dashboard';
import { AgentDashboard } from './features/agent/dashboard/dashboard';
import { Auth_Routes } from './features/auth/auth.route';

export const routes: Routes = [

    // Auth routes
    {path:'auth',children:Auth_Routes},

    //Role based Dashboard routes
    {path:'user/dashboard',component:UserDashboard,canActivate:[AuthGurad]},
    {path:'admin/dashboard',component:AdminDashboard,canActivate:[AuthGurad]},
    {path:'agent/dashboard',component:AgentDashboard,canActivate:[AuthGurad]},

    // Fallback route
    {path:'',redirectTo:'auth/login',pathMatch:'full'}
];
