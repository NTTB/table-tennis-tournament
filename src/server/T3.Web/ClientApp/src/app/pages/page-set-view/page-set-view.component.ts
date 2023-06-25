import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {map} from "rxjs";

@Component({
  selector: 'app-page-set-view',
  templateUrl: './page-set-view.component.html',
  styleUrls: ['./page-set-view.component.css']
})
export class PageSetViewComponent implements OnInit {

  constructor(private activatedRoute: ActivatedRoute) { }

  id$ = this.activatedRoute.params.pipe(map(x=> x['id']));

  ngOnInit(): void {
  }

}
