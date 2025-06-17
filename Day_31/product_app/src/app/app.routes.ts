import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { Products } from './components/products/products';
import { AuthGuard } from './guards/auth-guard';
import { ProductDetail } from './components/product-detail/product-detail';

export const routes: Routes = [
    {path:'',component:Login},
    {
        path:'products',
        canActivate:[AuthGuard],
        children:[
            {path:'',component:Products},
            {path:':id',component:ProductDetail}
        ]
    }
];
