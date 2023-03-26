import { Routes } from '@angular/router';
import {PageHomeComponent} from "./page-home/page-home.component";
import {PageLogoutComponent} from "./page-logout/page-logout.component";
import {PageCreateAccountComponent} from "./page-create-account/page-create-account.component";
import {PageLoginComponent} from "./page-login/page-login.component";

export const userRoutes: Routes = [
    // { path: 'status', component: PageStatusComponent },
    // { path: 'blog/page/:page', component: PageNewsComponent },
    // { path: 'blog', component: PageNewsComponent, pathMatch: 'full' },
    // { path: 'blog/:slug', component: PageNewsItemComponent },
    // { path: 'admin', loadChildren: () => import('./admin/admin.module').then(mod => mod.AdminModule), },
  { path: '', component: PageHomeComponent, pathMatch: "full" },
  { path: 'login', component: PageLoginComponent },
  { path: 'create-account', component: PageCreateAccountComponent },
  { path: 'logout', component: PageLogoutComponent },
    // { path: '**', component: PageContentComponent },
];
