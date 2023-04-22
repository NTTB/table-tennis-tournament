import {Component, Input, OnInit} from '@angular/core';
import {SetCommitService} from "../set-commit.service";
import {SetCommitBuilderService} from "../set-commit-builder.service";
import {SetCommitBody, SetCommitBodySetScoreChange} from "../models/set-commit-body";
import {SetView} from "../models/set-view";
import {filter, firstValueFrom, lastValueFrom, shareReplay} from "rxjs";

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

  setState$ = this.setCommitService.messages$.pipe(
    filter(x => x.header.setId.value == this.setId),
    shareReplay({refCount: true, bufferSize: 1}),
  );
  connectionState$ = this.setCommitService.state$;

  ngOnInit(): void {
    this.setCommitService.start().then(() => {
      this.setCommitService.addSetWatch({value: this.setId!}).then(() => console.log("Subscribed"));
    });
  }

  async sendNoOp() {
    if (!this.setId) return;

    let body: SetCommitBody = {type: 'NoOp'};
    let view: SetView = {
      gamesWon: {home: 0, away: 0},
    };

    const commit = await this.commitBuilder.create(this.setId, body, view, undefined);

    await this.setCommitService.push(commit);
  }

  async increaseSetScore(side: 'home' | 'away') {
    const setState = await firstValueFrom(this.setState$);
    let view: SetView = setState.view;
    view.gamesWon[side] += 1;
    let previousCommitId = setState.header.commitId?.value;
    let body: SetCommitBodySetScoreChange = {
      type: 'SetScoreChange', setScoreDelta: {
        home: side == 'home' ? 1 : 0,
        away: side == 'away' ? 1 : 0,
      }
    };

    const commit = await this.commitBuilder.create(this.setId!, body, view, previousCommitId);

    await this.setCommitService.push(commit);
  }
}
