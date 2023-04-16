import {Component, OnInit} from '@angular/core';
import {Router} from "@angular/router";

@Component({
  selector: 'app-page-login-post',
  templateUrl: './page-login-post.component.html',
  styleUrls: ['./page-login-post.component.css']
})
export class PageLoginPostComponent implements OnInit {

  constructor(private readonly router: Router) {
  }

  ngOnInit(): void {
  }

  async onUpdated() {
    await this.router.navigate(['/']);
  }
}
