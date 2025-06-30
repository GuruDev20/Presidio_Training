import { Routes } from '@angular/router';
import { SettingsComponent } from './features/settings/settings.component';

export const routes: Routes = [
    
    {path:'',redirectTo:'auth/login',pathMatch:'full'},
    {
        path:'auth',
        loadChildren:()=>import('./features/auth/auth-module').then(m=>m.AuthModule)
    },
    {
        path:'admin',
        loadChildren:()=>import('./features/admin/admin.routes').then(m=>m.Admin_Routes)
    },
    {
        path:'agent',
        loadChildren:()=>import('./features/agent/agent.routes').then(m=>m.Agent_Routes)
    },
    {
        path:'user',
        loadChildren:()=>import('./features/user/user.routes').then(m=>m.User_Routes)
    },
    {
        path:'chat',
        loadComponent:()=>import('./components/chat/chat.component').then(m=>m.ChatComponent)
    },
    {path:'settings',component:SettingsComponent}
];
