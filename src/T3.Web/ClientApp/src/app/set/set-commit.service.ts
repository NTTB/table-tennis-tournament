import {Injectable} from '@angular/core';
import {HubConnectionBuilder, HubConnectionState} from '@microsoft/signalr';
import {JwtService} from "../shared/jwt.service";
import {SetId} from "./models/typed-ids";
import {BehaviorSubject, Subject} from "rxjs";
import {SetCommit} from "./models/set-commit";

@Injectable({
  providedIn: 'root'
})
export class SetCommitService {
  private readonly connection = new HubConnectionBuilder()
    .withUrl("/hubs/set", {accessTokenFactory: () => this.jwtService.getToken() ?? ""})
    .withAutomaticReconnect()
    .build()
  ;

  private readonly _messages$ = new Subject<SetCommit>();
  public readonly messages$ = this._messages$.asObservable();

  private readonly _state$ = new BehaviorSubject<signalR.HubConnectionState>(this.connection.state);
  public readonly state$ = this._state$.asObservable();

  constructor(
    private readonly jwtService: JwtService
  ) {
  }

  public async start() {
    // Only execute this when the connection is disconnected
    if (this.connection.state !== "Disconnected") return;

    this.connection.onclose((error) => this._state$.next(this.connection.state));
    this.connection.onreconnected((connectionId) => this._state$.next(this.connection.state));
    this.connection.onreconnecting((error) => this._state$.next(this.connection.state));

    try {
      // Force the state to connecting to prevent multiple calls to start
      this._state$.next(HubConnectionState.Connecting);
      await this.connection.start();
    } catch (e) {
      this._state$.next(this.connection.state);
      throw e;
    }

    this._state$.next(this.connection.state);
    this.connection.on("SetCommitPushed", (setCommit: SetCommit) => {
      this._messages$.next(setCommit);
    });
  }

  public async stop() {
    // Close the connection
    await this.connection.stop();
  }

  /**
   * Send a set change to the server
   */
  public async push(commit: SetCommit) {
    await this.connection.invoke("Push", commit);
  }

  /**
   * Gets all set changes from the server of a specific set
   */
  public async getAll(setId: SetId) {
    return await this.connection.invoke<SetCommit[]>("GetAll", setId);
  }

  /**
   * Receive all future set changes from the server
   */
  public async addSetWatch(setId: SetId) {
    await this.connection.invoke("AddSetWatch", setId);
  }

  /**
   * Stop watching a set
   */
  public async removeSetWatch(setId: SetId) {
    await this.connection.invoke("RemoveSetWatch", setId);
  }

  /**
   * Watch all sets (most likely temporary)
   */
  public async addSetWatchAll() {
    await this.connection.invoke("AddSetWatchAll");
  }


  /**
   * Stop watching all sets (most likely temporary)
   */
  public async removeSetWatchAll() {
    await this.connection.invoke("RemoveSetWatchAll");
  }
}

