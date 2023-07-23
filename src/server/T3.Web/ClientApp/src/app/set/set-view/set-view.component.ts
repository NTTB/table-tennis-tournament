import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { BehaviorSubject, filter, firstValueFrom, map, ReplaySubject, shareReplay, Subject } from "rxjs";
import { v4 as uuidv4 } from 'uuid';
import {
  HubConnectionState,
  ISetHubSubscription,
  SetCommitMessage,
  PlayerView,
  SetCommit,
  SetCommitCommand,
  SetView,
  UpdateGameScoreCommand,
  UpdateSetScoreCommand,
  MessageBuilder,
  ClockService,
  SetsHub
} from '@nttb/t3-api-client';
import { IdentityService } from "../../account/identity.service";
import { KeyStorageService } from "../../account/key-storage.service";

@Component({
  selector: 'app-set-view',
  templateUrl: './set-view.component.html',
  styleUrls: ['./set-view.component.css']
})
export class SetViewComponent implements OnInit, OnDestroy {
  constructor(
    private readonly clockService: ClockService,
    private readonly setsHub: SetsHub,
    private readonly identityService: IdentityService,
    private readonly keyStorageService: KeyStorageService,
  ) {
  }

  @Input() setId?: string;

  ready$ = new BehaviorSubject<boolean>(false);

  messages$ = new ReplaySubject<SetCommitMessage>(1);
  setState$ = this.messages$.pipe(
    filter(x => x.content.header.setId.value == this.setId),
    shareReplay({ refCount: true, bufferSize: 1 }),
  );
  connectionState$ = new BehaviorSubject<HubConnectionState|undefined>(undefined);

  private _messageSubcription?: ISetHubSubscription;
  private _statusSubcription?: ISetHubSubscription;

  ngOnInit(): void {
    this._statusSubcription = this.setsHub.addStatusSubscription((state) => this.connectionState$.next(state));
    this.setsHub.start().then(() => {
      this._messageSubcription = this.setsHub.addMessageSubscription((msg) => this.messages$.next(msg));
      this.setsHub.addSetWatch({ value: this.setId! }).then(() => console.log("Subscribed"));
      return this.ready$.next(true);
    });
  }

  ngOnDestroy() {
    this.setsHub.removeSetWatch({ value: this.setId! }).then(() => console.log("Unsubscribed"));
    this._messageSubcription?.unsubscribe();
    this._statusSubcription?.unsubscribe();
  }

  async sendNoOp() {
    if (!this.setId) return;

    let content: SetCommit = {
      commands: [{ type: 'NoOp' }],
      header: {
        setId: { value: this.setId },
        commitId: { value: uuidv4() },
        createdAt: await this.clockService.getTimestamp(),
        previousCommitId: undefined,
        author: {
          userId: { value: this.identityService.userId! },
          sessionId: { value: this.identityService.stored.sessionId! },
          displayName: this.identityService.stored.displayName!,
        },
      },
      view: {
        gamesWon: { home: 0, away: 0 },
        games: [],
        setWatches: [],
        penaltyEvents: [],
      },
    };

    await this.pushMessage(content);
  }

  async sendInitialState() {
    if (!this.setId) return;
    var wouter: PlayerView = {
      displayName: "Wouter",
      playerId: { value: uuidv4() },
    };
    var rutger: PlayerView = {
      displayName: "Rutger",
      playerId: { value: uuidv4() },
    };

    let view: SetView = {
      games: [
        {
          initialServer: wouter.playerId,
          initialReceiver: rutger.playerId,
          currentServer: wouter.playerId,
          currentReceiver: rutger.playerId,
          points: { home: 0, away: 0 },
          watches: [],
        }
      ],
      homeTeam: { displayName: "home", players: [wouter], teamId: { value: uuidv4() } },
      awayTeam: { displayName: "away", players: [rutger], teamId: { value: uuidv4() } },
      gamesWon: { home: 0, away: 0 },
      setWatches: [],
      penaltyEvents: []
    };

    let setHomeTeamCommand: SetCommitCommand = {
      type: "SetHomeTeam",
      homeTeam: view.homeTeam,
    };
    let setAwayTeamCommand: SetCommitCommand = {
      type: "SetAwayTeam",
      awayTeam: view.awayTeam,
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

    var previousCommitId = await firstValueFrom(this.setState$.pipe(map(x => x.content.header.commitId)));

    let content: SetCommit = {
      commands: [
        setHomeTeamCommand,
        setAwayTeamCommand,
        addGameCommand,
        setInitialServerCommand,
        currentServerCommand
      ],
      header: {
        setId: { value: this.setId },
        commitId: { value: uuidv4() },
        createdAt: await this.clockService.getTimestamp(),
        previousCommitId: previousCommitId,
        author: {
          userId: { value: this.identityService.userId! },
          sessionId: { value: this.identityService.stored.sessionId! },
          displayName: this.identityService.stored.displayName!,
        },
      },
      view: view
    };


    await this.pushMessage(content);
  }

  async increaseSetScore(side: 'home' | 'away') {
    if (!this.setId) return;
    const setState = await firstValueFrom(this.setState$);
    let view: SetView = setState.content.view;
    view.gamesWon[side] += 1;
    let previousCommitId = setState.content.header.commitId!;
    let changeSetScoreCommand: UpdateSetScoreCommand = {
      type: 'UpdateSetScore', setScore: {
        home: view.gamesWon['home'],
        away: view.gamesWon['away'],
      }
    };

    let content: SetCommit = {
      commands: [
        changeSetScoreCommand,
      ],
      header: {
        setId: { value: this.setId },
        commitId: { value: uuidv4() },
        createdAt: await this.clockService.getTimestamp(),
        previousCommitId: previousCommitId,
        author: {
          userId: { value: this.identityService.userId! },
          sessionId: { value: this.identityService.stored.sessionId! },
          displayName: this.identityService.stored.displayName!,
        },
      },
      view: view
    };

    await this.pushMessage(content);
  }

  private async pushMessage(content: SetCommit) {
    let builder = new MessageBuilder();
    let cryptoKeyPair: CryptoKeyPair = {
      privateKey: (await this.keyStorageService.getPrivateKey())!,
      publicKey: (await this.keyStorageService.getPublicKey())!
    };
    let message = await builder.buildMessage(content, cryptoKeyPair);
    await this.setsHub.push(message);
  }

  async addGame() {
    if (!this.setId) return;
    const setState = await firstValueFrom(this.setState$);
    let view: SetView = setState.content.view;
    let previousCommitId = setState.content.header.commitId;
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
      points: { home: 0, away: 0 },
      watches: [],
    });

    let content: SetCommit = {
      commands: [
        addGameCommand,
        setInitialServerCommand,
        setCurrentServerCommand
      ],
      header: {
        setId: { value: this.setId },
        commitId: { value: uuidv4() },
        createdAt: await this.clockService.getTimestamp(),
        previousCommitId: previousCommitId,
        author: {
          userId: { value: this.identityService.userId! },
          sessionId: { value: this.identityService.stored.sessionId! },
          displayName: this.identityService.stored.displayName!,
        },
      },
      view: view
    };

    await this.pushMessage(content);
  }

  async increaseGameScore(side: 'home' | 'away') {
    if (!this.setId) return;
    const setState = await firstValueFrom(this.setState$);
    let view: SetView = setState.content.view;
    if (view.games.length == 0) {
      throw new Error("No games in set");
    }

    let gameIndex = view.games.length - 1;

    if (gameIndex < 0 || gameIndex >= view.games.length) {
      throw new Error("Invalid game index");
    }

    view.games[gameIndex].points[side] += 1;

    let previousCommitId = setState.content.header.commitId!;
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

    let content: SetCommit = {
      commands: [
        ...commands,
      ],
      header: {
        setId: { value: this.setId },
        commitId: { value: uuidv4() },
        createdAt: await this.clockService.getTimestamp(),
        previousCommitId: previousCommitId,
        author: {
          userId: { value: this.identityService.userId! },
          sessionId: { value: this.identityService.stored.sessionId! },
          displayName: this.identityService.stored.displayName!,
        },
      },
      view: view
    };

    await this.pushMessage(content);
  }
}
