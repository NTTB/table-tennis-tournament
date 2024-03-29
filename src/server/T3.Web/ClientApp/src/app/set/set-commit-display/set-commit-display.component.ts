import { Component, Input, OnInit } from '@angular/core';
import { SetApi, SetCommit, SetCommitValidationResult } from "@nttb/t3-api-client";
import { addDays, addMilliseconds, format } from 'date-fns';

/**
 * Shows a set commit in a human readable way.
 */
@Component({
  selector: 'app-set-commit-display',
  templateUrl: './set-commit-display.component.html',
  styleUrls: ['./set-commit-display.component.css']
})
export class SetCommitDisplayComponent implements OnInit {
  @Input()
  setCommit!: SetCommit;
  validationResult?: SetCommitValidationResult;

  get header() {
    return this.setCommit.header;
  }

  get author() {
    return this.header.author;
  }

  get clientApp() {
    return this.author.clientInfo;
  }

  get view() {
    return this.setCommit.view;
  }

  get createdAtDateTime() {
    let serverTimestamp = this.header.createdAt.serverTimestamp;
    let date = new Date(serverTimestamp.year, 0);
    date = addDays(date, serverTimestamp.dayOfYear - 1);
    date = addMilliseconds(date, serverTimestamp.millisecondOfDay);
    date = addMilliseconds(date, this.header.createdAt.clientOffset.milliseconds);
    const str = format(date, "yyyy-MM-dd'T'HH:mm:ss.SSSSSS");
    return new Date(str + '+0000');
  }

  constructor(
    private readonly setApi: SetApi,
  ) {
  }

  ngOnInit(): void {
  }

  async onValidateClick() {
    this.validationResult = await this.setApi.validate(this.setCommit.header.setId, this.setCommit.header.commitId);
  }
}
