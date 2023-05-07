import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {SetCommitService} from "../set-commit.service";
import {SetCommitBuilderService} from "../set-commit-builder.service";
import {SetCommitCommand, UpdateGameScoreCommand, UpdateSetScoreCommand} from "../models/set-commit-command";
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
      games: [],
    };

    const commit = await this.commitBuilder.create(this.setId, [noOpCommand], view, undefined);

    await this.setCommitService.push(commit);
  }

  async sendInitialState() {
    var wouter: PlayerView = {
      displayName: "Wouter",
      playerId: {value: uuidv4()},
    };
    var rutger: PlayerView = {
      displayName: "Rutger",
      playerId: {value: uuidv4()},
    };

    let view: SetView = {
      games: [
        {
          initialServer: wouter.playerId,
          initialReceiver: rutger.playerId,
          currentServer: wouter.playerId,
          currentReceiver: rutger.playerId,
          points: {home: 0, away: 0}
        }
      ],
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

    let addGameCommand: SetCommitCommand = {
      type: "AddGame",
      position: 0,
      amount: 1,
    };

    let setInitialServerCommand: SetCommitCommand = {
      type: "SetInitialServer",
      gameIndex: 0,
      servingPlayer: wouter.playerId,
      receivingPlayer: rutger.playerId,
    };

    let currentServerCommand: SetCommitCommand = {
      type: "SetCurrentServer",
      gameIndex: 0,
      servingPlayer: wouter.playerId,
      receivingPlayer: rutger.playerId,
    };

    const commit = await this.commitBuilder.create(this.setId!, [
      setHomePlayersCommand,
      setAwayPlayersCommand,
      addGameCommand,
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
    let changeSetScoreCommand: UpdateSetScoreCommand = {
      type: 'UpdateSetScore', setScore: {
        home: view.gamesWon['home'],
        away: view.gamesWon['away'],
      }
    };

    const commit = await this.commitBuilder.create(this.setId!, [changeSetScoreCommand], view, previousCommitId);

    await this.setCommitService.push(commit);
  }

  async addGame() {
    const setState = await firstValueFrom(this.setState$);
    let view: SetView = setState.view;
    let previousCommitId = setState.header.commitId?.value;
    let addGameCommand: SetCommitCommand = {
      type: 'AddGame',
      position: view.games.length,
      amount: 1,
    };

    var nextServer = view.games[view.games.length - 1].currentReceiver;
    var nextReceiver = view.games[view.games.length - 1].currentServer;

    let setInitialServerCommand: SetCommitCommand = {
      type: "SetInitialServer",
      gameIndex: view.games.length,
      servingPlayer: nextServer!,
      receivingPlayer: nextReceiver!,
    };

    let setCurrentServerCommand: SetCommitCommand = {
      type: "SetCurrentServer",
      gameIndex: view.games.length,
      servingPlayer: nextServer!,
      receivingPlayer: nextReceiver!,
    };

    view.games.push({
      initialServer: nextServer,
      initialReceiver: nextReceiver,
      currentServer: nextServer,
      currentReceiver: nextReceiver,
      points: {home: 0, away: 0}
    });

    const commit = await this.commitBuilder.create(this.setId!, [addGameCommand, setInitialServerCommand, setCurrentServerCommand], view, previousCommitId);

    await this.setCommitService.push(commit);
  }

  async increaseGameScore(side: 'home' | 'away') {
    const setState = await firstValueFrom(this.setState$);
    let view: SetView = setState.view;
    if (view.games.length == 0) {
      throw new Error("No games in set");
    }

    let gameIndex = view.games.length - 1;

    if (gameIndex < 0 || gameIndex >= view.games.length) {
      throw new Error("Invalid game index");
    }

    view.games[gameIndex].points[side] += 1;

    let previousCommitId = setState.header.commitId?.value;
    let updateGameScoreCommand: UpdateGameScoreCommand = {
      type: 'UpdateGameScore',
      gameIndex: gameIndex,
      gameScore: {
        home: view.games[gameIndex].points['home'],
        away: view.games[gameIndex].points['away'],
      }
    };
    let commands: SetCommitCommand[] = [updateGameScoreCommand];

    // Do we need to change the server?
    var totalPoints = view.games[gameIndex].points['home'] + view.games[gameIndex].points['away'];
    if (totalPoints % 2 == 0) {
      var nextServer = view.games[gameIndex].currentReceiver;
      var nextReceiver = view.games[gameIndex].currentServer;

      let setCurrentServerCommand: SetCommitCommand = {
        type: "SetCurrentServer",
        gameIndex: gameIndex,
        servingPlayer: nextServer!,
        receivingPlayer: nextReceiver!,
      };

      // Update the state
      view.games[gameIndex].currentServer = nextServer;
      view.games[gameIndex].currentReceiver = nextReceiver;

      // Add the command
      commands.push(setCurrentServerCommand);
    }

    const commit = await this.commitBuilder.create(this.setId!, commands, view, previousCommitId);
    await this.setCommitService.push(commit);
  }
}
