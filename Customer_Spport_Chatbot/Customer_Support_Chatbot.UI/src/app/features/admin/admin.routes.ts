import { Routes } from "@angular/router";
import { AdminDashboard } from "./overview/overview";

export const Admin_Routes:Routes=[
    {
        path:'dashboard',
        children:[
            {
                path:'overview',
                component:AdminDashboard
            }
        ]
    }
]