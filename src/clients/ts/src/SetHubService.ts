import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { IStartable } from "./IStartable";
import { IStoppable } from "./IStoppable";
import { IJwtTokenProvider } from "./IJwtTokenProvider";
import { SetCommitMessage } from "./models/SetCommitMessage";
import { SetId } from "./models/typed-ids";

export interface ISetHubService extends IStartable, IStoppable {
}

export class SetHubService implements ISetHubService {
  private readonly _connection: HubConnection;

  constructor(hubsSetUrl: string, jwtTokenProvider: IJwtTokenProvider) {
    this._connection = new HubConnectionBuilder()
      .withUrl(hubsSetUrl, {
        accessTokenFactory: async () => (await jwtTokenProvider.getJwtToken()) ?? "",
      })
      .withAutomaticReconnect()
      .build();
  }

  async start(): Promise<void> {
    await this._connection.start();
  }

  async stop(): Promise<void> {
    await this._connection.stop();
  }

  async push(msg: SetCommitMessage): Promise<void> {
    await this._connection.invoke("Push", msg);
  }

  /**
   * Gets all set changes from the server of a specific set
   */
  public async getAll(setId: SetId) {
    return await this._connection.invoke<SetCommitMessage[]>("GetAll", setId);
  }

  /**
   * Receive all future set changes from the server
   */
  public async addSetWatch(setId: SetId) {
    await this._connection.invoke("AddSetWatch", setId);
  }

  /**
   * Stop watching a set
   */
  public async removeSetWatch(setId: SetId) {
    await this._connection.invoke("RemoveSetWatch", setId);
  }

  /**
   * Watch all sets (most likely temporary)
   */
  public async addSetWatchAll() {
    await this._connection.invoke("AddSetWatchAll");
  }

  /**
   * Stop watching all sets (most likely temporary)
   */
  public async removeSetWatchAll() {
    await this._connection.invoke("RemoveSetWatchAll");
  }
}