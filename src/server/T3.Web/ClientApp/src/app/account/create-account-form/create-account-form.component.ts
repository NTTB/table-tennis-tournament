import {startWith, tap, catchError, NEVER, EMPTY, BehaviorSubject, map} from 'rxjs';
import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {FormGroup, FormControl, Validators, ValidationErrors, AbstractControl} from '@angular/forms';
import {AccountApiService} from "../account-api.service";
import {Router} from '@angular/router';

@Component({
  selector: 'app-create-account-form',
  templateUrl: './create-account-form.component.html',
  styleUrls: ['./create-account-form.component.css']
})
export class CreateAccountFormComponent implements OnInit {
  constructor(
    private readonly http: HttpClient,
    @Inject('BASE_URL') private readonly baseUrl: string,
    private readonly accountApi: AccountApiService,
    private readonly router: Router,
  ) {
  }

  readonly form = new FormGroup(
    {
      username: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required),
      password_confirmation: new FormControl('', Validators.required),
    },
    {
      validators: (formGroup): ValidationErrors => {
        const password = formGroup.get('password')?.value;
        const password_confirmation = formGroup.get('password_confirmation')?.value;
        if (password !== password_confirmation) {
          return {passwordsDontMatch: true};
        }
        return {};
      }
    }
  );

  readonly username = this.form.get("username")!;
  readonly password = this.form.get("password")!;
  readonly password_confirmation = this.form.get("password_confirmation")!;

  readonly error$ = new BehaviorSubject<string | null>(null);
  isFormDisabled$ = this.form.statusChanges.pipe(
    startWith(this.form.status),
    map(x => x === 'INVALID')
  );


  ngOnInit(): void {
  }

  onSubmit() {
    this.error$.next(null);
    if (this.form.invalid) {
      return;
    }

    var formData = this.form.value;
    this.accountApi.create(formData.username!, formData.password!).pipe(
      // tap(res => this.jwtService.setToken(res.jwtToken)),
      tap(() => this.router.navigate(['/'])),
      catchError(err => {
        this.error$.next(JSON.stringify(err, null, 2));
        return NEVER;
      }),
    ).subscribe();
  }

  hasError(control: AbstractControl, validation: string): Boolean {
    return (control.errors?.[validation]) ?? false;
  }
}
