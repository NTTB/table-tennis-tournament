import {Component, OnInit} from '@angular/core';
import {decodeJwt} from 'jose';
import {map} from 'rxjs';
import {JwtService} from "../../../shared/jwt.service";

@Component({
  selector: 'app-front-page-header',
  templateUrl: './front-page-header.component.html',
  styleUrls: ['./front-page-header.component.css']
})
export class FrontPageHeaderComponent implements OnInit {

  constructor(
    private readonly jwtService: JwtService,
  ) {
  }

  jwt$ = this.jwtService.jwt$.pipe(
    map(x => x ? decodeJwt(x) : null)
  );

  username$ = this.jwt$.pipe(map(x => x ? x['name'] : null));
  userId$ = this.jwt$.pipe(map(x => x ? x.sub : null));

  ngOnInit(): void {

  }

}
