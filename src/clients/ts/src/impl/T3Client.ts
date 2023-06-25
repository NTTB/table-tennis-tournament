import { LocalStorageDataStore } from "./LocalStorageDataStore";
import { Config } from "../config";
import { IT3Client } from "../IT3Client";
import { IDataStore } from "../IDataStore";
import { ISetCommitReceiverCollection } from "../ISetCommitReceiverManager";
import { SetCommitReceiverCollection } from "./SetCommitReceiverManager";
import { HubConnectionBuilder, HubConnection, IHttpConnectionOptions } from "@microsoft/signalr";

export class T3Client implements IT3Client {
  private _dataStore: IDataStore = new LocalStorageDataStore();
  private _connection: HubConnection;

  public get dataStore(): IDataStore { return this._dataStore; }
  public set dataStore(value: IDataStore) { this._dataStore = value; }

  public readonly setCommitReceiverCollection: ISetCommitReceiverCollection = new SetCommitReceiverCollection();

  constructor(
    private readonly config: Config
  ) {
    if (config.overrides?.dataStore) {
      this._dataStore = config.overrides.dataStore;
    }

    const newLocal = "/hubs/set";
    const newLocal_1: IHttpConnectionOptions = { accessTokenFactory: () => this.config.getJwtToken() ?? "", };
    this._connection = new HubConnectionBuilder()
      .withUrl(newLocal, newLocal_1)
      .withAutomaticReconnect()
      .build()
  }

  async start() {
    // TODO: Setup the streams, start clock-service
    throw new Error('Not implemented');
  }

  async stop() {
    throw new Error('Not implemented');
  }

}