import { IDataStore,  LocalStorageDataStore} from "./DataStore";
import { IJwtTokenProvider } from "./IJwtTokenProvider";
import { HttpClient, IHttpClient } from "./HttpClient";
import { ClockService, IClockService } from "./ClockService";
import { IStoppable } from "./IStoppable";
import { IStartable } from "./IStartable";
import { ISetHubService, SetHubService } from "./SetHubService";

/**
 * Client used to communicate with the server about sets.
 */
export interface ISetClient {
  /**
   * Start the client and connect.
   */
  start(): Promise<void>;

  /**
   * Stop the client and disconnect.
   */
  stop(): Promise<void>;
}

export class SetClient implements ISetClient {
  private _startables: IStartable[] = [];
  private _stoppables: IStoppable[] = [];

  private _dataStore: IDataStore = new LocalStorageDataStore();
  private _httpClient: IHttpClient;
  private _clockService: IClockService;
  private _hubsSet: ISetHubService;


  public get dataStore(): IDataStore { return this._dataStore; }
  public set dataStore(value: IDataStore) { this._dataStore = value; }
  
  private registerServices<T extends (IStartable | IStoppable)>(service: T) {
    if ((service as IStartable).start) {
      this._startables.push(service as IStartable);
    }
    if ((service as IStoppable).stop) {
      this._stoppables.push(service as IStoppable);
    }
    return service;
  }

  constructor(
    baseUrl: string,
     jwtTokenProvider: IJwtTokenProvider,
  ) {
    if (baseUrl == null) throw new Error("baseUrl cannot be null");
    if (jwtTokenProvider == null) throw new Error("jwtTokenProvider cannot be null");

    if (baseUrl.endsWith("/")) throw new Error("baseUrl cannot end with a slash");

    this._httpClient = new HttpClient(baseUrl, jwtTokenProvider);
    this._clockService = this.registerServices(new ClockService(this._httpClient));
    const hubsSetUrl = baseUrl + "/hubs/set";
    this._hubsSet = this.registerServices(new SetHubService(hubsSetUrl, jwtTokenProvider));
  }

  async start() {
    await Promise.all(this._startables.map(s => s.start()));
  }

  async stop() {
    await Promise.all(this._stoppables.map(s => s.stop()));
  }

  
}