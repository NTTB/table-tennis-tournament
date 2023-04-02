import {Component, OnInit} from '@angular/core';
import {SetApiService, SetEntity} from "../set-api.service";
import {tap, BehaviorSubject, switchMap, lastValueFrom,} from "rxjs";

@Component({
  selector: 'app-set-list',
  templateUrl: './set-list.component.html',
  styleUrls: ['./set-list.component.css']
})
export class SetListComponent implements OnInit {

  constructor(
    private readonly setApiService: SetApiService
  ) {
  }

  reload$ = new BehaviorSubject(null);
  sets$ = this.reload$.pipe(switchMap(_ => this.setApiService.getAll()));

  ngOnInit(): void {
  }

  async delete(set: SetEntity) {
    await lastValueFrom(this.setApiService.delete(set.id).pipe(
      tap(() => {
        this.reload$.next(null);
      }),
    ))
  }
}
