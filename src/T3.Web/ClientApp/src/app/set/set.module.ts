import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SetFormComponent } from './set-form/set-form.component';
import { SetListComponent } from './set-list/set-list.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";



@NgModule({
  declarations: [
    SetFormComponent,
    SetListComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
  ],
  exports: [
    SetFormComponent,
    SetListComponent
  ]
})
export class SetModule { }
