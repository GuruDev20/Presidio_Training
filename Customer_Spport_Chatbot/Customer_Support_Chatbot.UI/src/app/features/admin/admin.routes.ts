import { Routes } from "@angular/router";
import { AdminLayoutComponent } from "../../layout/admin-layout/admin-layout.component";
import { AdminDashboard } from "./overview/overview";
import { Admin_Sidebar_Constants } from "../../constants/admin-sidebar.constant";
import { AdminWorkspaceComponent } from "./workspace/workspace.component";
import { Profile } from "../../components/profile/profile.component";
import { SettingsComponent } from "../settings/settings.component";
import { AdminSettingsComponent } from "../../components/settings/admin-settings/admin-settings.component";

export const Admin_Routes:Routes=[
    {
        path:'dashboard',
        component:AdminLayoutComponent,
        data:{items:Admin_Sidebar_Constants},
        children:[
            {path:'', redirectTo:'overview', pathMatch:'full'},
            {path:'overview',component:AdminDashboard},
            {path:'workspace',component:AdminWorkspaceComponent},
            {path:'profile',component:Profile},
            {path:'settings',component:AdminSettingsComponent}
        ]
    }
]