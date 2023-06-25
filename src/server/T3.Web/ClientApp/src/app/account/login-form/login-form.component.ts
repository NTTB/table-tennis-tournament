import {tap, catchError, NEVER, BehaviorSubject, lastValueFrom} from 'rxjs';
import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {FormGroup, FormControl} from '@angular/forms';
import {AccountApiService} from "../account-api.service";
import {JwtService} from "../../shared/jwt.service";
import {CryptoKeyService} from '../crypto-key.service';
import {KeyStorageService} from '../key-storage.service';
import {IdentityService} from "../identity.service";
import {SetCommitBuilderService} from "../../set/set-commit-builder.service";

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.css']
})
export class LoginFormComponent implements OnInit {
  constructor(
    private readonly accountApi: AccountApiService,
    private readonly jwtService: JwtService,
    private readonly router: Router,
    private readonly cryptoKeyService: CryptoKeyService,
    private readonly keyStorageService: KeyStorageService,
    private readonly identityService: IdentityService,
    private readonly setCommitBuilderService: SetCommitBuilderService,
  ) {
  }

  form = new FormGroup({
    username: new FormControl(''),
    password: new FormControl(''),
  });

  error$ = new BehaviorSubject<string | null>(null);

  ngOnInit(): void {
  }

  onSubmit() {
    const formData = this.form.value;
    this.accountApi.login(formData.username!, formData.password!).pipe(
      tap(res => this.jwtService.setToken(res.jwtToken)),
      tap(async () => {
        var pair = await this.cryptoKeyService.generateKey();
        await this.keyStorageService.set(pair);
        // Send public key to server
        await lastValueFrom(this.accountApi.addKey({
          publicKey: JSON.stringify(await this.cryptoKeyService.serialize(pair.publicKey))
        }));
      }),
      tap(() => {
        const userId = this.identityService.userId;
        if (userId === undefined) throw new Error("No user id");

        // Perform post login actions
        this.setCommitBuilderService.updateAuthor({
          displayName: this.identityService.stored.displayName,
          sessionId: this.identityService.userId ? {value: this.identityService.userId} : undefined,
          userId: {value: userId},
          deviceName: this.identityService.stored.deviceName,
        });
      }),
      tap(() => this.router.navigate(['/account/post-login'])),
      catchError(err => {
        this.error$.next(JSON.stringify(err, null, 2));
        return NEVER;
      }),
    ).subscribe();
  }
}
