import {NgModule} from '@angular/core';
import {HttpClientModule} from '@angular/common/http';
import {CommonModule} from '@angular/common';
import {FormsModule} from "@angular/forms";
import {ApiService} from "./api.service";


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [
    ApiService,
  ],
})
export class SharedModule {
}
