import { Routes } from '@angular/router';
import { SettingsComponent } from './features/settings/settings.component';
import { HomeComponent } from './features/home/home.component';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  {
    path: 'auth',
    loadChildren: () =>
      import('./features/auth/auth-module').then((m) => m.AuthModule),
  },
  {
    path: 'admin',
    loadChildren: () =>
      import('./features/admin/admin.routes').then((m) => m.Admin_Routes),
  },
  {
    path: 'agent',
    loadChildren: () =>
      import('./features/agent/agent.routes').then((m) => m.Agent_Routes),
  },
  {
    path: 'user',
    loadChildren: () =>
      import('./features/user/user.routes').then((m) => m.User_Routes),
  },
];
