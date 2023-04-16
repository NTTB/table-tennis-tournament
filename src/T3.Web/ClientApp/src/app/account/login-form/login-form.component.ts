import { tap, catchError, NEVER, BehaviorSubject, lastValueFrom } from 'rxjs';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormControl } from '@angular/forms';
import { AccountApiService } from "../account-api.service";
import { JwtService } from "../../shared/jwt.service";
import { CryptoKeyService } from '../crypto-key.service';
import { KeyStorageService } from '../key-storage.service';

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
      tap(() => this.router.navigate(['/'])),
      catchError(err => {
        this.error$.next(JSON.stringify(err, null, 2));
        return NEVER;
      }),
    ).subscribe();
  }
}
