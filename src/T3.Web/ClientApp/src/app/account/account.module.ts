import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";

import {SharedModule} from "../shared/shared.module";

import {AccountApiService} from "./account-api.service";

import {LoginFormComponent} from './login-form/login-form.component';
import {CreateAccountFormComponent} from './create-account-form/create-account-form.component';
import { AccountKeysComponent } from './account-keys/account-keys.component';

@NgModule({
  declarations: [
    CreateAccountFormComponent,
    LoginFormComponent,
    AccountKeysComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule
  ],
  providers: [
    AccountApiService,
  ],
  exports: [
    CreateAccountFormComponent,
    AccountKeysComponent,
    LoginFormComponent,
  ]
})
export class AccountModule {
}
