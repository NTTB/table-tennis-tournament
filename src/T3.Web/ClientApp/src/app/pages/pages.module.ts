import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterModule} from '@angular/router';

import {AccountModule} from "../account/account.module";
import {SetModule} from "../set/set.module";

import {FrontPageHeaderComponent} from './shared/front-page-header/front-page-header.component';
import {PageCreateAccountComponent} from './page-create-account/page-create-account.component';
import {PageHomeComponent} from './page-home/page-home.component';
import {PageLoginComponent} from './page-login/page-login.component';
import {PageLogoutComponent} from './page-logout/page-logout.component';
import {PageSetCreateComponent} from './page-set-create/page-set-create.component';
import {PageSetListComponent} from './page-set-list/page-set-list.component';
import { PageSetViewComponent } from './page-set-view/page-set-view.component';
import { PageAccountKeysComponent } from './page-account-keys/page-account-keys.component';
import { PageLoginPostComponent } from './page-login-post/page-login-post.component';


@NgModule({
  declarations: [
    PageHomeComponent,
    PageLoginComponent,
    PageCreateAccountComponent,
    PageLogoutComponent,
    FrontPageHeaderComponent,
    PageSetCreateComponent,
    PageSetListComponent,
    PageSetViewComponent,
    PageAccountKeysComponent,
    PageLoginPostComponent,
  ],
  imports: [
    CommonModule,
    RouterModule,
    AccountModule,
    SetModule,
  ]
})
export class PagesModule {
}
