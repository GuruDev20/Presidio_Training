import { Routes } from '@angular/router';
import { Dashboard } from './components/dashboard/dashboard';
import { AddUser } from './components/add-user/add-user';

export const routes: Routes = [
    {path:'',redirectTo:'/dashboard',pathMatch:'full'},
    {path:'dashboard',component:Dashboard},
    {path:'add-user',component:AddUser}
];
     