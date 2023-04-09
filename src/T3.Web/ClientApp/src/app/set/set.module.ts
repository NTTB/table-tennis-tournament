import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SetFormComponent } from './set-form/set-form.component';
import { SetListComponent } from './set-list/set-list.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import { SetViewComponent } from './set-view/set-view.component';
import { RouterModule } from '@angular/router';



@NgModule({
  declarations: [
    SetFormComponent,
    SetListComponent,
    SetViewComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    ReactiveFormsModule,
  ],
  exports: [
    SetFormComponent,
    SetListComponent,
    SetViewComponent
  ]
})
export class SetModule { }
