import { HubConnection, HubConnectionBuilder, HubConnectionState as SignalrConnectionState } from "@microsoft/signalr";
import { IJwtTokenProvider } from "./IJwtTokenProvider";
import { SetCommitMessage } from "./models/SetCommitMessage";
import { SetId } from "./models/typed-ids";
import { HubConnectionState } from "./HubConnectionState";

export interface ISetHubSubscription {
  token: Symbol;
  /**
   * Unsubscribe, checks if the subscription is still valid
   */
  unsubscribe(): void;
}

class SetHubSubscription implements ISetHubSubscription {
  constructor(public readonly token: Symbol, private readonly setHub: WeakRef<ISetsHub>) { }
  unsubscribe(): void {
    this.setHub.deref()?.removeSubscription(this);
  }
}

export interface ISetsHub {

  /**
   * Start the connection to the server
   */
  start(): Promise<void>;

  /**
   * Stop the connection to the server
   */
  stop(): Promise<void>;

  /**
   * Push a message to the server, which will be broadcast to all clients
   * @param msg The message to push to the server
   */
  push(msg: SetCommitMessage): Promise<void>;

  /**
   * Gets all set changes from the server of a specific set
   * @param setId The id of the set to get all changes for
   */
  getAll(setId: SetId): Promise<SetCommitMessage[]>;

  /**
   * Receive all future set changes from the server
   * @param setId The id of the set to watch
   */
  addSetWatch(setId: SetId): Promise<void>;

  /**
   * Stop watching a set
   * @param setId The id of the set to stop watching
   */
  removeSetWatch(setId: SetId): Promise<void>;

  /**
   * Watch all sets (most likely temporary)
   */
  addSetWatchAll(): Promise<void>;

  /**
   * Stop watching all sets (most likely temporary)
   */
  removeSetWatchAll(): Promise<void>;

  /**
   * Unsubscribe a subscription, can be used for any subscription
   * @param token The token returned from addSubscription
   */
  removeSubscription(subscription: ISetHubSubscription): void;

  /**
   * Subscribe to the SetHub's OnSetChanged event
   * @param callback The callback to invoke when the event is raised
   */
  addMessageSubscription(callback: (msg: SetCommitMessage) => any): ISetHubSubscription;


  /**
   * Subscribe to the status of the SetHub
   * @param callback The callback to invoke when the event is raised
   */
  addStatusSubscription(callback: (status: HubConnectionState, error: Error | undefined) => any): ISetHubSubscription;
}

export class SetsHub implements ISetsHub {
  private readonly _connection: HubConnection;
  private readonly _messageSubscriptions: Map<Symbol, (msg: SetCommitMessage) => any> = new Map();
  private readonly _statusSubscriptions: Map<Symbol, (status: HubConnectionState, error: Error | undefined) => any> = new Map();
  private _subscriptionCalls = 0;

  constructor(baseUrl: string, jwtTokenProvider: IJwtTokenProvider) {
    if (baseUrl.endsWith("/")) throw new Error("baseUrl cannot end with a slash");
    if (jwtTokenProvider == null) throw new Error("jwtTokenProvider cannot be null");

    this._connection = new HubConnectionBuilder()
      .withUrl(baseUrl, {
        accessTokenFactory: async () => (await jwtTokenProvider.getJwtToken()) ?? "",
      })
      .withAutomaticReconnect()
      .build();

    this._connection.onclose((error) => this._onStatus(this._connection.state, error));
    this._connection.onreconnected((connectionId) => this._onStatus(this._connection.state, undefined));
    this._connection.onreconnecting((error) => this._onStatus(this._connection.state, error));

    this._connection.on("SetCommitPushed", (setCommit: SetCommitMessage) => {
      this._onMessage(setCommit);
    });
  }

  async start(): Promise<void> {
    /**
     * If the connection is already connected, don't try to connect again
     */
    if (this._connection.state === SignalrConnectionState.Connected) return;
    const promise = this._connection.start();
    this._onStatus(this._connection.state, undefined);

    try {
      await promise;
    } catch (e) {
      this._onStatus(this._connection.state, e as Error);
      throw e;
    }

    this._onStatus(this._connection.state, undefined);
  }

  async stop(): Promise<void> {
    await this._connection.stop();
  }

  async push(msg: SetCommitMessage): Promise<void> {
    await this._connection.invoke("Push", msg);
  }

  public async getAll(setId: SetId) {
    return await this._connection.invoke<SetCommitMessage[]>("GetAll", setId);
  }

  public async addSetWatch(setId: SetId) {
    await this._connection.invoke("AddSetWatch", setId);
  }

  public async removeSetWatch(setId: SetId) {
    await this._connection.invoke("RemoveSetWatch", setId);
  }

  public async addSetWatchAll() {
    await this._connection.invoke("AddSetWatchAll");
  }

  public async removeSetWatchAll() {
    await this._connection.invoke("RemoveSetWatchAll");
  }

  public removeSubscription(subscription: ISetHubSubscription): void {
    this._messageSubscriptions.delete(subscription.token);
    this._statusSubscriptions.delete(subscription.token);
  }

  public addMessageSubscription(callback: (msg: SetCommitMessage) => any): ISetHubSubscription {
    const registrationSymbol = Symbol("SetHubService.addSubscription_" + this._subscriptionCalls++);
    this._messageSubscriptions.set(registrationSymbol, callback);
    return new SetHubSubscription(registrationSymbol, new WeakRef<ISetsHub>(this));
  }

  public addStatusSubscription(callback: (status: HubConnectionState, error: Error | undefined) => any): ISetHubSubscription {
    const registrationSymbol = Symbol("SetHubService.addStatusSubscription_" + this._subscriptionCalls++);
    this._statusSubscriptions.set(registrationSymbol, callback);
    return new SetHubSubscription(registrationSymbol, new WeakRef<ISetsHub>(this));
  }

  private _onMessage(msg: SetCommitMessage) {
    const callbacks = this._messageSubscriptions.values();
    for (const callback of callbacks) {
      try {
        callback(msg);
      } catch (e) {
        console.error("An error occured when invoking a callback", e);
      }
    }
  }

  private _onStatus(status: HubConnectionState, error: Error | undefined) {
    const callbacks = this._statusSubscriptions.values();
    for (const callback of callbacks) {
      try {
        callback(status, error);
      } catch (e) {
        console.error("An error occured when invoking a callback", e);
      }
    }
  }
}