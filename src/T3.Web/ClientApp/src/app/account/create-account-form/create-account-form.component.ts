import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-create-account-form',
  templateUrl: './create-account-form.component.html',
  styleUrls: ['./create-account-form.component.css']
})
export class CreateAccountFormComponent implements OnInit {
  username: string = "";
  password: string = "";
  password_confirmation: string = "";

  constructor(
    private readonly http: HttpClient,
    @Inject('BASE_URL') private readonly baseUrl: string
  ) {
  }

  ngOnInit(): void {
  }

  onSubmit() {
    console.log("submitting");
    this.http.post(this.baseUrl + "api/account/create", {
      username: this.username,
      password: this.password,
    }).subscribe(x => console.log(x));
  }

}
