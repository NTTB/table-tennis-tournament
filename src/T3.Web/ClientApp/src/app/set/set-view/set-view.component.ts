import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {SetCommitService} from "../set-commit.service";
import {SetCommitBuilderService} from "../set-commit-builder.service";
import {SetCommitCommand, ChangeSetScoreCommand} from "../models/set-commit-command";
import {SetView} from "../models/set-view";
import {filter, firstValueFrom, shareReplay} from "rxjs";
import {PlayerView} from "../models/player-view";
import {v4 as uuidv4} from 'uuid';

@Component({
  selector: 'app-set-view',
  templateUrl: './set-view.component.html',
  styleUrls: ['./set-view.component.css']
})
export class SetViewComponent implements OnInit, OnDestroy {
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

  ngOnDestroy() {
    this.setCommitService.removeSetWatch({value: this.setId!}).then(() => console.log("Unsubscribed"));
  }

  async sendNoOp() {
    if (!this.setId) return;

    let noOpCommand: SetCommitCommand = {type: 'NoOp'};
    let view: SetView = {
      gamesWon: {home: 0, away: 0},
      homePlayers: [],
      awayPlayers: [],
      currentServer: undefined,
      currentReceiver: undefined,
      initialReceiver: undefined,
      initialServer: undefined,
    };

    const commit = await this.commitBuilder.create(this.setId, [noOpCommand], view, undefined);

    await this.setCommitService.push(commit);
  }

  async sendInitialState(){
    var wouter: PlayerView = {
      displayName: "Wouter",
      playerId: {value: uuidv4()},
    };
    var rutger: PlayerView = {
      displayName: "Rutger",
      playerId: {value: uuidv4()},
    };

    let view: SetView = {
      initialServer: wouter,
      initialReceiver: rutger,
      currentServer: wouter,
      currentReceiver: rutger,
      homePlayers: [wouter],
      awayPlayers: [rutger],
      gamesWon: {home: 0, away: 0},
    };

    let setHomePlayersCommand: SetCommitCommand = {
      type: "SetHomePlayers",
      homePlayers: [wouter],
    };
    let setAwayPlayersCommand: SetCommitCommand = {
      type: "SetAwayPlayers",
      awayPlayers: [rutger],
    };

    let setInitialServerCommand: SetCommitCommand = {
      type: "SetInitialService",
      servingPlayer: wouter.playerId,
      receivingPlayer: rutger.playerId,
    };

    let currentServerCommand: SetCommitCommand = {
      type: "SetCurrentService",
      servingPlayer: wouter.playerId,
      receivingPlayer: rutger.playerId,
    };

    const commit = await this.commitBuilder.create(this.setId!, [
      setHomePlayersCommand,
      setAwayPlayersCommand,
      setInitialServerCommand,
      currentServerCommand
    ], view, undefined);

    await this.setCommitService.push(commit);
  }

  async increaseSetScore(side: 'home' | 'away') {
    const setState = await firstValueFrom(this.setState$);
    let view: SetView = setState.view;
    view.gamesWon[side] += 1;
    let previousCommitId = setState.header.commitId?.value;
    let changeSetScoreCommand: ChangeSetScoreCommand = {
      type: 'SetScoreChange', setScoreDelta: {
        home: side == 'home' ? 1 : 0,
        away: side == 'away' ? 1 : 0,
      }
    };

    const commit = await this.commitBuilder.create(this.setId!, [changeSetScoreCommand], view, previousCommitId);

    await this.setCommitService.push(commit);
  }
}
