import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.css']
})
export class LoginFormComponent implements OnInit {
  username: string = "";
  password: string = "";

  constructor(
    private readonly http: HttpClient,
    @Inject('BASE_URL') private readonly baseUrl: string
  ) {
  }

  ngOnInit(): void {
  }

  onSubmit() {
    console.log("submitting");
    this.http.post<{jwtToken:string}>(this.baseUrl + "api/account/login", {
      username: this.username,
      password: this.password,
    }).subscribe(x => {
      console.log(x);
      localStorage.setItem('token', x.jwtToken);
    });
  }
}
