import { Login } from "./login/login";
import { Register } from "./register/register";
import { Routes } from "@angular/router";

export const Auth_Routes:Routes=[
    {path:'login',component:Login},
    {path:'register',component:Register}
]