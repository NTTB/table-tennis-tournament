import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageHomeComponent } from './page-home/page-home.component';
import { RouterModule } from '@angular/router';
import { PageLoginComponent } from './page-login/page-login.component';
import { PageCreateAccountComponent } from './page-create-account/page-create-account.component';
import { PageLogoutComponent } from './page-logout/page-logout.component';
import {AccountModule} from "../account/account.module";
import { FrontPageHeaderComponent } from './shared/front-page-header/front-page-header.component';



@NgModule({
  declarations: [
    PageHomeComponent,
    PageLoginComponent,
    PageCreateAccountComponent,
    PageLogoutComponent,
    FrontPageHeaderComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    AccountModule,
  ]
})
export class PagesModule { }
