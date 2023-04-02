import {Component, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import {lastValueFrom} from "rxjs";
import {SetApiService} from "../../set/set-api.service";
import {SetFormData} from "../../set/set-form/set-form.component";

@Component({
  selector: 'app-page-set-create',
  templateUrl: './page-set-create.component.html',
  styleUrls: ['./page-set-create.component.css']
})
export class PageSetCreateComponent implements OnInit {

  constructor(
    private readonly router: Router,
    private readonly setApi: SetApiService,
  ) {
  }

  ngOnInit(): void {
  }

  async OnSubmitClicked(ev: SetFormData) {
    await lastValueFrom(this.setApi.create({
      displayName: ev.displayName
    }));

    await this.router.navigate(['/sets']);
  }
}
