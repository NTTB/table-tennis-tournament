import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, ValidationErrors, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountApi } from '@nttb/t3-api-client';
import { startWith, BehaviorSubject, map } from 'rxjs';

@Component({
  selector: 'app-create-account-form',
  templateUrl: './create-account-form.component.html',
  styleUrls: ['./create-account-form.component.css']
})
export class CreateAccountFormComponent implements OnInit {
  constructor(
    private readonly accountApi: AccountApi,
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
          return { passwordsDontMatch: true };
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

  async onSubmit() {
    this.error$.next(null);
    if (this.form.invalid) {
      return;
    }

    var formData = this.form.value;

    try {
      await this.accountApi.create(formData.username!, formData.password!);
      this.router.navigate(['/']);

    } catch (err) {
      this.error$.next(JSON.stringify(err, null, 2));
    }
  }

  hasError(control: AbstractControl, validation: string): Boolean {
    return (control.errors?.[validation]) ?? false;
  }
}
