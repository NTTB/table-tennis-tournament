import { Component, OnInit } from '@angular/core';
import { AccountApi, AccountKeysResponse } from '@nttb/t3-api-client';
import { BehaviorSubject, switchMap } from "rxjs";


@Component({
  selector: 'app-account-keys',
  templateUrl: './account-keys.component.html',
  styleUrls: ['./account-keys.component.css']
})
export class AccountKeysComponent implements OnInit {
  constructor(
    private readonly accountApi: AccountApi,
  ) {
  }

  reload$ = new BehaviorSubject<void>(undefined);
  keys$ = this.reload$.pipe(
    switchMap(() => this.accountApi.getKeys())
  );

  ngOnInit(): void {
  }

  revokeKey(item: AccountKeysResponse) {
    this.accountApi
      .revokeKey({ publicKey: item.publicKey })
      .then(() => this.reload$.next());
  }
}
