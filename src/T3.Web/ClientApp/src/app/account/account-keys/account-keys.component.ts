import {Component, OnInit} from '@angular/core';
import {BehaviorSubject, switchMap} from "rxjs";
import {AccountApiService, AccountKeysResponse} from "../account-api.service";

@Component({
  selector: 'app-account-keys',
  templateUrl: './account-keys.component.html',
  styleUrls: ['./account-keys.component.css']
})
export class AccountKeysComponent implements OnInit {
  constructor(
    private readonly accountApiService: AccountApiService,
  ) {
  }

  reload$ = new BehaviorSubject<void>(undefined);
  keys$ = this.reload$.pipe(
    switchMap(() => this.accountApiService.getKeys())
  );

  ngOnInit(): void {
  }

  revokeKey(item: AccountKeysResponse) {
    this.accountApiService
      .revokeKey({publicKey: item.publicKey})
      .subscribe(() => this.reload$.next());
  }
}
