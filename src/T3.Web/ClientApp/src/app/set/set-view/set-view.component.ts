import {Component, Input, OnInit} from '@angular/core';
import {SetCommitService} from "../set-commit.service";
import {SetCommitBuilderService} from "../set-commit-builder.service";
import {SetCommitBody} from "../models/set-commit-body";
import {SetView} from "../models/set-view";

@Component({
  selector: 'app-set-view',
  templateUrl: './set-view.component.html',
  styleUrls: ['./set-view.component.css']
})
export class SetViewComponent implements OnInit {
  constructor(
    private readonly setCommitService: SetCommitService,
    private readonly commitBuilder: SetCommitBuilderService,
  ) {
  }

  @Input() setId?: string;

  lastMessage$ = this.setCommitService.messages$;
  lastState$ = this.setCommitService.state$;

  ngOnInit(): void {
    this.setCommitService.start().then(() => {
      this.setCommitService.addSetWatch({value: this.setId!}).then(() => console.log("Subscribed"));

    });
  }

  async sendUpdate() {
    if (!this.setId) return;

    let body: SetCommitBody = {type: 'NoOp'};
    let view: SetView = {
      gamesWon: {home: 0, away: 0},
    };

    const commit = await this.commitBuilder.create(this.setId, body, view, undefined);

    await this.setCommitService.push(commit);
  }

}
