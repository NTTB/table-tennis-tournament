import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";

import {SharedModule} from "../shared/shared.module";


import {LoginFormComponent} from './login-form/login-form.component';
import {CreateAccountFormComponent} from './create-account-form/create-account-form.component';
import { AccountKeysComponent } from './account-keys/account-keys.component';
import { PostLoginFormComponent } from './post-login-form/post-login-form.component';

@NgModule({
  declarations: [
    CreateAccountFormComponent,
    LoginFormComponent,
    AccountKeysComponent,
    PostLoginFormComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule
  ],
  exports: [
    CreateAccountFormComponent,
    AccountKeysComponent,
    LoginFormComponent,
    PostLoginFormComponent,
  ]
})
export class AccountModule {
}
