import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {JwtService} from "../../shared/jwt.service";

@Component({
  selector: 'app-page-logout',
  templateUrl: './page-logout.component.html',
  styleUrls: ['./page-logout.component.css']
})
export class PageLogoutComponent implements OnInit {

  constructor(
    private readonly router: Router,
    private readonly jwtService: JwtService,
  ) {
  }

  ngOnInit(): void {
  }

  logout() {
    this.jwtService.destroyToken();
    this.router.navigate(['/']);
  }

  cancel() {
    this.router.navigate(['/']);
  }
}
