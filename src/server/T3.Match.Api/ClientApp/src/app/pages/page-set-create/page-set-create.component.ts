import { Component, OnInit } from '@angular/core';
import { Router } from "@angular/router";
import { SetFormData } from "../../set/set-form/set-form.component";
import { SetApi } from '@nttb/t3-api-client';

@Component({
  selector: 'app-page-set-create',
  templateUrl: './page-set-create.component.html',
  styleUrls: ['./page-set-create.component.css']
})
export class PageSetCreateComponent implements OnInit {

  constructor(
    private readonly router: Router,
    private readonly setApi: SetApi,
  ) {
  }

  ngOnInit(): void {
  }

  async OnSubmitClicked(ev: SetFormData) {
    await this.setApi.create({
      displayName: ev.displayName
    });

    await this.router.navigate(['/sets']);
  }
}
