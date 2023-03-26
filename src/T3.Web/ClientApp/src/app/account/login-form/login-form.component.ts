import {tap, catchError, NEVER, BehaviorSubject} from 'rxjs';
import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {FormGroup, FormControl} from '@angular/forms';
import {AccountApiService} from "../account-api.service";
import {JwtService} from "../../shared/jwt.service";

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
      tap(() => this.router.navigate(['/'])),
      catchError(err => {
        this.error$.next(JSON.stringify(err, null, 2));
        return NEVER;
      }),
    ).subscribe();
  }
}
