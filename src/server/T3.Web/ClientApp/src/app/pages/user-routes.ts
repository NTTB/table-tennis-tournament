import {Routes} from '@angular/router';
import {PageHomeComponent} from "./page-home/page-home.component";
import {PageLogoutComponent} from "./page-logout/page-logout.component";
import {PageCreateAccountComponent} from "./page-create-account/page-create-account.component";
import {PageLoginComponent} from "./page-login/page-login.component";
import {PageSetCreateComponent} from "./page-set-create/page-set-create.component";
import {PageSetListComponent} from "./page-set-list/page-set-list.component";
import {PageSetViewComponent} from "./page-set-view/page-set-view.component";
import {PageAccountKeysComponent} from "./page-account-keys/page-account-keys.component";
import {PageLoginPostComponent} from "./page-login-post/page-login-post.component";
import {PageTestSecretKeysComponent} from "./page-test-secret-keys/page-test-secret-keys.component";

export const userRoutes: Routes = [
  // { path: 'status', component: PageStatusComponent },
  // { path: 'blog/page/:page', component: PageNewsComponent },
  // { path: 'blog', component: PageNewsComponent, pathMatch: 'full' },
  // { path: 'blog/:slug', component: PageNewsItemComponent },
  // { path: 'admin', loadChildren: () => import('./admin/admin.module').then(mod => mod.AdminModule), },
  {path: '', component: PageHomeComponent, pathMatch: "full"},
  {path: 'login', component: PageLoginComponent},
  {path: 'create-account', component: PageCreateAccountComponent},
  {path: 'logout', component: PageLogoutComponent},
  {path: 'account/keys', component: PageAccountKeysComponent},
  {path: 'account/post-login', component: PageLoginPostComponent},

  {path: 'sets', component: PageSetListComponent},
  {path: 'sets/create', component: PageSetCreateComponent},
  {path: 'sets/:id', component: PageSetViewComponent},

  {path: 'test/secret-keys', component: PageTestSecretKeysComponent},

  // { path: '**', component: PageContentComponent },
];
