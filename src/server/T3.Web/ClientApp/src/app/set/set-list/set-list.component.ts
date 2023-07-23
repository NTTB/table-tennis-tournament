import { Component, OnInit } from '@angular/core';
import { SetEntity, SetApi } from '@nttb/t3-api-client';
import { BehaviorSubject, switchMap } from "rxjs";

@Component({
  selector: 'app-set-list',
  templateUrl: './set-list.component.html',
  styleUrls: ['./set-list.component.css']
})
export class SetListComponent implements OnInit {

  constructor(
    private readonly setApi: SetApi,
  ) {
  }

  reload$ = new BehaviorSubject(null);
  sets$ = this.reload$.pipe(switchMap(_ => this.setApi.getAll()));

  ngOnInit(): void {
  }

  async delete(set: SetEntity) {
    await this.setApi.delete(set.id);
    this.reload$.next(null);
  }
}
