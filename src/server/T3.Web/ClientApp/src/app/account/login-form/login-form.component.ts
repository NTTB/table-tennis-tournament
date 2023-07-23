import { BehaviorSubject } from 'rxjs';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormControl } from '@angular/forms';
import { JwtService } from "../../shared/jwt.service";
import { AccountApi } from '@nttb/t3-api-client';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.css']
})
export class LoginFormComponent implements OnInit {
  constructor(
    private readonly jwtService: JwtService,
    private readonly router: Router,
    private readonly accountApi: AccountApi,
  ) {
  }

  form = new FormGroup({
    username: new FormControl('', { nonNullable: true }),
    password: new FormControl('', { nonNullable: true }),
  });

  error$ = new BehaviorSubject<string | null>(null);

  ngOnInit(): void {
  }

  async onSubmit() {
    const formData = this.form.value;
    if (formData.username === undefined) throw new Error("Username is undefined");
    if (formData.password === undefined) throw new Error("Password is undefined");

    try {
      var result = await this.accountApi.login(formData.username, formData.password);
      this.jwtService.setToken(result.jwtToken);

      // Navigate to post login page
      this.router.navigate(['/account/post-login']);
    } catch (err) {
      this.error$.next(JSON.stringify(err, null, 2));
    }
  }
}
